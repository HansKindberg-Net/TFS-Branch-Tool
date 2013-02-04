// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccCreateFolderTests.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The scc create folder tests.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions;
    using Microsoft.ALMRangers.BranchTool.SourceControlWrapper;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.TeamFoundation.Client.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The create folder tests.
    /// </summary>
    [TestClass]
    public class SccCreateFolderTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create folder execute async test.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task.
        /// </returns>
        [TestMethod]
        public async Task SccCreateFolderExecuteAsyncTest()
        {
            using (ShimsContext.Create())
            {
                const string CollectionUrl = "http://FakeServer:8080/tfs/TestCollection";

                ShimTfsTeamProjectCollection.ConstructorUri = (@this, value) =>
                {
                    // ReSharper disable ObjectCreationAsStatement
                    new ShimTfsTeamProjectCollection(@this) { NameGet = () => CollectionUrl };
                    new ShimTfsConnection(@this) { EnsureAuthenticated = () => { } };
                    // ReSharper restore ObjectCreationAsStatement
                };

                var activity = new SccCreateFolder
                    { FolderPath = @"$\Main", SourceControl = new MockSourceControlWrapper() };

                activity.SourceControl.CreateWorkspace();

                await activity.ExecuteAsync(new ActionExecutionContext());
            }
        }

        #endregion
    }

    public class MockSourceControlWrapper : ISourceControlWrapper
    {
        public void Commit(string checkInComment, string overridePolicyComment)
        {
        }

        public void CreateWorkspace()
        {
        }

        public void CreateFolder(string s, bool failIfNotParentExist)
        {
        }

        public void CreateBranch(string source, string target)
        {
        }

        public bool FolderExist(string folderpath)
        {
            throw new NotImplementedException();
        }
    }
}