// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="SourceControlWrapper.cs">
//   Copyright © Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.SourceControlWrapper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    public class SourceControlWrapper : ISourceControlWrapper
    {
        private readonly TfsTeamProjectCollection tpcollection;
        private readonly VersionControlServer vcserver;
        private readonly string teamProjectName;
        private string tmpWsRootFolder;
        private string uniqueId;
        private Workspace tmpWs;

        public SourceControlWrapper(string teamProjectCollectionUrl, string teamProjectName)
        {
            this.tpcollection = new TfsTeamProjectCollection(new Uri(teamProjectCollectionUrl));
            this.teamProjectName = teamProjectName;

            this.vcserver = this.tpcollection.GetService<VersionControlServer>();
        }

        ~SourceControlWrapper()
        {
            if (this.tmpWs != null)
            {
                if (this.tmpWs.VersionControlServer != null)
                {
                    this.tmpWs.Delete();
                }

                this.tmpWs = null;
            }

            if (this.tmpWsRootFolder != null)
            {
                var dir = new DirectoryInfo(this.tmpWsRootFolder);

                if (dir.Exists)
                {
                    dir.Delete(true);
                }

                this.tmpWsRootFolder = null;
            }
        }

        public void CreateWorkspace()
        {
            this.uniqueId = Guid.NewGuid().ToString();

            // Using temporary directory for the workspace mapping
            var dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), this.uniqueId));
         
            if (!dir.Exists)
            {
                dir.Create();
            }

            this.tmpWsRootFolder = dir.FullName;

            this.tpcollection.EnsureAuthenticated();

            string username = this.GetUserName();

            // Query for workspaces and delete if found.
            var ws = this.vcserver.QueryWorkspaces("temp" + this.uniqueId, username, Environment.MachineName).FirstOrDefault();
            if (ws != null)
            {
                ws.Delete();
            }

            // Create the workspace with a mapping to the temporary folder.
            this.tmpWs = this.vcserver.CreateWorkspace("temp" + this.uniqueId, username, string.Empty, new[] { new WorkingFolder(this.RootFolder(), dir.FullName) });
        }

        public void Commit(string checkinComment, string commentOverRidePolicy)
        {
            // Finally check-in, don't trigger a Continuous Integration build and override gated check-in.
            var wip = new WorkspaceCheckInParameters(this.tmpWs.GetPendingChanges(), "***NO_CI***" + checkinComment)
            {
                // Enable the override of gated check-in when the server supports gated check-ins.
                OverrideGatedCheckIn = ((CheckInOptions2)this.vcserver.SupportedFeatures & CheckInOptions2.OverrideGatedCheckIn) == CheckInOptions2.OverrideGatedCheckIn,
                PolicyOverride = new PolicyOverrideInfo(commentOverRidePolicy, null)
            };
            
            this.tmpWs.CheckIn(wip);
        }

        public void CreateFolder(string serverPath, bool onlyCreatLastFolderInPath)
        {
            string relativPath = serverPath.Replace(this.RootFolder() + "/", string.Empty);

            if (onlyCreatLastFolderInPath)
            {
                string parentDirToLastFolder = serverPath.Substring(0, serverPath.LastIndexOf("/"));

                if (!this.FolderExist(parentDirToLastFolder))
                {
                    throw new InvalidDataException(
                        string.Format("The folder '{0}' doesn't exist! '.", parentDirToLastFolder));
                }
            }

            var dir = new DirectoryInfo(Path.Combine(this.tmpWsRootFolder, relativPath));
            
            if (!dir.Exists)
            {
                dir.Create();
            }

            this.tmpWs.PendAdd(dir.FullName);
        }

        public void CreateBranch(string branchSource, string branchTarget)
        {
            ItemIdentifier sourceItm = new ItemIdentifier(branchSource);
            if (this.vcserver.QueryBranchObjects(sourceItm, RecursionType.None).Length == 0)
            {
                this.vcserver.CreateBranchObject(new BranchProperties(sourceItm));
            }

            this.tmpWs.PendBranch(branchSource, branchTarget, VersionSpec.Latest);
        }

        public bool FileExist(string fileName)
        {
            return this.vcserver.ServerItemExists(fileName, ItemType.File);
        }

        public bool FolderExist(string folderpath)
        {
            return this.vcserver.ServerItemExists(folderpath, ItemType.Folder);
        }

        private string RootFolder()
        {
            return this.vcserver.GetTeamProject(this.teamProjectName).ServerItem;
        }

        private string GetUserName()
        {
            string username = null;
           
            if (this.tpcollection.Credentials != null)
            {
                var networkCredential = this.tpcollection.Credentials as NetworkCredential;
                if (networkCredential != null)
                {
                    username = networkCredential.Domain + @"\" + networkCredential.UserName;
                }
            }

            if (username == null || username == "\\")
            {
                if (this.tpcollection.AuthorizedIdentity != null)
                {
                    if (this.tpcollection.AuthorizedIdentity.UniqueName != string.Empty)
                    {
                        username = this.tpcollection.AuthorizedIdentity.UniqueName;
                }
            }
            }
        
            return username ?? Environment.UserName;
        }
    }
}