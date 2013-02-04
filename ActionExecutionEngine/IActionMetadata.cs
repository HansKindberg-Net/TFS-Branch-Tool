// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IActionMetadata.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The ActionMetadata interface.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System.ComponentModel;

    /// <summary>
    /// The ActionMetadata interface.
    /// </summary>
    public interface IActionMetadata
    {
        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [DefaultValue(1)]
        int Version { get; }

        #endregion
    }
}