// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccConnect.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines SccConnect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Microsoft.ALMRangers.BranchTool.SourceControlWrapper;

    /// <summary>
    /// Establishes connection to source control subsystem.
    /// </summary>
    [Export(typeof(IAction))]
    [ExportMetadata("Name", "SccConnect")]
    [ExportMetadata("Version", 1)]
    public class SccConnect : SccActionBase, IAction
    {
        #region Public Properties
        /// <summary>The Uri to the Team project collection.</summary>
        public string Collection { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }

        /// <summary>Gets the team project name.</summary>
        public string TeamProject
        {
            get
            {
                string s = this.Path.Replace(@"$/", string.Empty);
                if (s.IndexOf(@"/", System.StringComparison.Ordinal) > 0)
                {
                    s = s.Substring(0, s.IndexOf(@"/", System.StringComparison.Ordinal));
                }

                return s;
            }
        }
        #endregion

        #region Public Methods and Operators

        /// <summary>Asynchronously establishes connection to source control subsystem.</summary>
        /// <param name="context">The <see cref="IActionExecutionContext"/> that represents execution context.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            var t = new Task(
                () =>
                    {
                        this.SourceControl = new SourceControlWrapper(this.Collection, this.TeamProject);
                        this.SourceControl.CreateWorkspace();
                    });
            t.Start();
            return t;
        }

        #endregion
    }
}