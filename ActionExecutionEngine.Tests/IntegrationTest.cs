// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the IntegrationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions;
    using Microsoft.ALMRangers.BranchTool.SourceControlWrapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IntegrationTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create folder execute async test.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task.
        /// </returns>
        [TestMethod]
        [Ignore]
        public async Task ActivityIntegrationTest()
        {
            const string CollectionUrl = @"http://sogbotfsvs3:8080/tfs/PlayAround";
            const string ProjectName = "MattiasDemo2011";

            ISourceControlWrapper wrapper = new SourceControlWrapper(CollectionUrl, ProjectName);
            wrapper.CreateWorkspace();

            var activity = new SccCreateFolder { FolderPath = @"$/MattiasDemo2011/Main4", SourceControl = wrapper };

            var context = new ActionExecutionContext();
            await activity.ExecuteAsync(context);

            var checkin = new SccCheckIn { SourceControl = wrapper };

            await checkin.ExecuteAsync(context);

            var branch = new SccCreateBranch
                {
                    BranchSource = @"$/MattiasDemo2011/Main4",
                    BranchTarget = @"$/MattiasDemo2011/Dev4",
                    SourceControl = wrapper
                };

            await branch.ExecuteAsync(context);

            var checkin2 = new SccCheckIn { SourceControl = wrapper };

            await checkin2.ExecuteAsync(context);
        }

        #endregion
    }
}