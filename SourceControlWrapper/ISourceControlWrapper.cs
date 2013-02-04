// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISourceControlWrapper.cs" company="Microsoft Corporation">
//   Copyright © Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The ViewModel of the BranchPlan
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.SourceControlWrapper
{
    public interface ISourceControlWrapper
    {
        void CreateWorkspace();

        void CreateFolder(string serverPath, bool onlyCreatLastFolderInPath);

        void Commit(string checkInComment, string overridePolicyComment);

        void CreateBranch(string branchSource, string branchTarget);
      
        bool FolderExist(string folderpath);
    }
}
