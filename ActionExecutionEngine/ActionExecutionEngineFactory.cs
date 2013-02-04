// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionExecutionEngineFactory.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines action execution engine factory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    /// <summary>Action execution engine factory.</summary>
    public static class ActionExecutionEngineFactory
    {
        #region Public Methods and Operators

        /// <summary>Creates new instance of action execution engine.</summary>
        /// <returns>The <see cref="IActionExecutionEngine"/>.</returns>
        public static IActionExecutionEngine CreateActionExecutionEngine()
        {
            return new ActionExecutionEngine();
        }

        #endregion
    }
}