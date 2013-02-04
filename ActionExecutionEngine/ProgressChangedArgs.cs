// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressChangedArgs.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The progress changed args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;

    /// <summary>The progress changed args.</summary>
    public class ProgressChangedArgs : EventArgs
    {
        #region Public Properties

        /// <summary>Gets or sets the operation.</summary>
        public string Message { get; set; }

        /// <summary>Gets or sets the progress amount.</summary>
        public int Progress { get; set; }

        #endregion
    }
}