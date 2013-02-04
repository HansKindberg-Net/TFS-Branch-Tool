// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccFolderExist.cs">
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
    [ExportMetadata("Name", "SccFolderExist")]
    [ExportMetadata("Version", 1)]
    public class SccFolderExist : SccActionBase, IAction
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SccFolderExist"/> class.</summary>
        public SccFolderExist()
        {
            this.FolderPath = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the created folder path.</summary>
        public string FolderPath { private get; set; }

        /// <summary>The result of the operation.</summary>
        public bool Result { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute async.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/></returns>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            Task t = new Task(() => this.Result = SourceControl.FolderExist(this.FolderPath));
            t.Start();
            return t;
        }

        #endregion
    }
}