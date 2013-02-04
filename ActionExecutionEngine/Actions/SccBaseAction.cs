// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SccBaseAction.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Definition of the SccActionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Actions
{
    using Microsoft.ALMRangers.BranchTool.SourceControlWrapper;

    /// <summary>
    /// Base class for all source control actions.
    /// </summary>
    public abstract class SccActionBase
    {
        #region Public Properties
        /// <summary>Gets or sets <see cref="ISourceControlWrapper"/>.</summary>
        public ISourceControlWrapper SourceControl { get; set; }

        #endregion
    }
}