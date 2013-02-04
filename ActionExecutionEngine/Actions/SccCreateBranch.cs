// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccCreateBranch.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The scc create folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>
    /// Create branch
    /// </summary>
    [Export(typeof(IAction))]
    [ExportMetadata("Name", "SccCreateBranch")]
    [ExportMetadata("Version", 1)]
    public class SccCreateBranch : SccActionBase, IAction
    {
        #region Public Properties

        /// <summary>Gets the branch path.</summary>
        public string BranchPath { get; private set; }

        /// <summary>Gets or sets the branch source.</summary>
        public string BranchSource { private get; set; }

        /// <summary>Gets or sets the branch target.</summary>
        public string BranchTarget { private get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute async.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The System.Threading.Tasks.Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            Task t = new Task(() => SourceControl.CreateBranch(this.BranchSource, this.BranchTarget));
            t.Start();
            return t;
        }

        #endregion
    }
}