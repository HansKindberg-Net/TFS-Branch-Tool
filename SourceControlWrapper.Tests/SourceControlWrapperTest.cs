// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceControlWrapperTest.cs" company="Microsoft Corporation">
//   Copyright © Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The source control wrapper test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.SourceControlWrapper.Tests
{
    using System.IO.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.TeamFoundation.Client.Fakes;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Client.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The source control wrapper test.</summary>
    [TestClass]
    public class SourceControlWrapperTest
    {
        #region Public Methods and Operators

        /// <summary>The commit test.</summary>
        [TestMethod]
        public void CommitTest()
        {
            const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";
            const string TeamProjectName = "TestProject";
            const int ChangesetId = 123;

            using (ShimsContext.Create())
            {
                var checkInCalled = false;
                ShimWorkspace.AllInstances.CheckInWorkspaceCheckInParameters = (@this, x) => { checkInCalled = true; return ChangesetId; }; 
                this.SetupTfsShim(CollectionUrl, TeamProjectName, ChangesetId, 3);

                var sc = new SourceControlWrapper(CollectionUrl, TeamProjectName);
                sc.CreateWorkspace();
                sc.Commit("comment", "OverRide Comment");
        
                Assert.IsTrue(checkInCalled);
            }
        }

        /// <summary>The create branch test.</summary>
        [TestMethod]
        public void CreateBranchTest()
        {
            const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";
            const string TeamProjectName = "TestProject";
            const int PendingChangeCount = 1;

            using (ShimsContext.Create())
            {
                var createBranchObjectCalled = false;
                ShimVersionControlServer.AllInstances.CreateBranchObjectBranchProperties = (@this, x) => { createBranchObjectCalled = true; };
                var pendBranchCalled = false;
                ShimWorkspace.AllInstances.PendBranchStringStringVersionSpec = (@this, x, y, z) => { pendBranchCalled = true; return PendingChangeCount; };
                this.SetupTfsShim(CollectionUrl, TeamProjectName, 123, PendingChangeCount);

                var sc = new SourceControlWrapper(CollectionUrl, TeamProjectName);
                sc.CreateWorkspace();
                sc.CreateBranch("Main", "Dev");

                Assert.IsTrue(createBranchObjectCalled);
                Assert.IsTrue(pendBranchCalled);
            }
        }

        /// <summary>The create folder test.</summary>
        [TestMethod]
        public void CreateFolderTest()
        {
            const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";
            const string TeamProjectName = "TestProject";
            const int PendingChangeCount = 1;

            using (ShimsContext.Create())
            {
                var pendAddCalled = false;

                ShimWorkspace.AllInstances.PendAddString = (@this, x) => { pendAddCalled = true; return PendingChangeCount; };
                this.SetupTfsShim(CollectionUrl, TeamProjectName, 123, PendingChangeCount);

                var sc = new SourceControlWrapper(CollectionUrl, TeamProjectName);
                sc.CreateWorkspace();
                
                ShimDirectoryInfo.ConstructorString = (@this, value) =>
                {
                    new ShimDirectoryInfo(@this)
                    {
                        ExistsGet = () => false,
                        Create = () => { },
                    };
                };
                
                ShimDirectory.GetDemandDirStringBoolean = (fullPath, thisDirOnly) => System.IO.Path.GetTempPath();
                
                sc.CreateFolder("$/TestProject/Main/Folder", true);
                
                Assert.IsTrue(pendAddCalled);
            }
        }

        /// <summary>The create workspace with fakes test.</summary>
        [TestMethod]
        public void CreateWorkspaceTest()
        {
            const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";
            const string TeamProjectName = "TestProject";

            using (ShimsContext.Create())
            {
                this.SetupTfsShim(CollectionUrl, TeamProjectName, 123, 1);

                var sc = new SourceControlWrapper(CollectionUrl, TeamProjectName);
                sc.CreateWorkspace();
            }
        }

        /// <summary>The file exists test.</summary>
        [TestMethod]
        public void FileExistsTest()
        {
            const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";
            const string TeamProjectName = "TestProject";

            using (ShimsContext.Create())
            {
                this.SetupTfsShim(CollectionUrl, TeamProjectName, 123, 3);

                var sc = new SourceControlWrapper(CollectionUrl, TeamProjectName);
                sc.CreateWorkspace();
                var result = sc.FileExist("testfile");
                Assert.IsTrue(result, "Incorrect result for FileExists");
            }
        }

        #endregion

        #region Methods

        /// <summary>The setup TFS shim.</summary>
        /// <param name="collectionUrl">The collection URL.</param>
        /// <param name="teamProjectName">The team project name.</param>
        /// <param name="changesetId">The change set id.</param>
        /// <param name="pendingChangeCount">The pending change count.</param>
        private void SetupTfsShim(
            string collectionUrl, string teamProjectName, int changesetId, int pendingChangeCount)
        {
            ShimTfsTeamProjectCollection.ConstructorUri = (@this, value) =>
                {
// ReSharper disable ObjectCreationAsStatement
                    new ShimTfsTeamProjectCollection(@this) { NameGet = () => collectionUrl };
                    new ShimTfsConnection(@this) 
                    {
                        EnsureAuthenticated = () => { },
                        CredentialsGet = () => new System.Net.NetworkCredential("testUser", "pwd", "testDomain")
                    };
// ReSharper restore ObjectCreationAsStatement
                };

            var workspace = new ShimWorkspace
                {
                    CheckInPendingChangeArrayString = (x, y) => changesetId,
                    Delete = () => true,
                    GetPendingChanges = () => new PendingChange[pendingChangeCount],
                };

            var teamProject = new ShimTeamProject { ServerItemGet = () => string.Format("$/{0}", teamProjectName) };

            var versionControlServer = new ShimVersionControlServer
                {
                    CreateWorkspaceStringStringStringWorkingFolderArray = (a, b, c, d) => workspace,
                    QueryWorkspacesStringStringString = (a, b, c) => new Workspace[0],
                    GetTeamProjectString = a => teamProject,
                    QueryBranchObjectsItemIdentifierRecursionType = (a, b) => new BranchObject[0],
                    SupportedFeaturesGet = () => 8,
                    ServerItemExistsStringItemType = (x, y) => true
                };

            ShimTfsConnection.AllInstances.GetServiceOf1<VersionControlServer>(t => versionControlServer);
        }

        #endregion
    }
}