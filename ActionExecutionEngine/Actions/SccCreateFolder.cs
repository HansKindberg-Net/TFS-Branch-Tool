// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccCreateFolder.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The scc create folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>
    /// The create folder.
    /// </summary>
    [Export(typeof(IAction))]
    [ExportMetadata("Name", "SccCreateFolder")]
    [ExportMetadata("Version", 1)]
    public class SccCreateFolder : SccActionBase, IAction
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SccCreateFolder"/> class.</summary>
        public SccCreateFolder()
        {
            this.FolderPath = string.Empty;
            this.OnlyCreateLastFolderInPath = false;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the created folder path.</summary>
        public string FolderPath { private get; set; }

        /// <summary>Determins if the Action will fail or creata folders if folders in the FolderPath doesnt exist</summary>
        public bool OnlyCreateLastFolderInPath { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute async.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/></returns>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            Task t = new Task(() => SourceControl.CreateFolder(this.FolderPath, this.OnlyCreateLastFolderInPath));
            t.Start();
            return t;
        }

        #endregion
    }
}