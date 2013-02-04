// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccCheckIn.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Definition of the SccCheckIn type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>
    /// Source control check-in action.
    /// </summary>
    [Export(typeof(IAction))]
    [ExportMetadata("Name", "SccCheckIn")]
    [ExportMetadata("Version", 1)]
    public class SccCheckIn : SccActionBase, IAction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SccCheckIn"/> class. 
        /// </summary>
        public SccCheckIn()
        {
            this.OverridePolicyComment = string.Empty;
            this.CheckinComment = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the check-in comment.</summary>
        public string CheckinComment { get; set; }

        /// <summary>Gets or sets the override policy comment.</summary>
        public string OverridePolicyComment { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Asynchronously performs check-in to source control.</summary>
        /// <param name="context">The <see cref="IActionExecutionContext"/> that represents execution context.</param>
        /// <returns><see cref="Task"/></returns>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            var t = new Task(() => this.SourceControl.Commit(this.CheckinComment, this.OverridePolicyComment));
            
            t.Start();

            return t;
        }

        #endregion
    }
}