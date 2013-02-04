// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IActionExecutionEngine.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The ActionExecutionEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Action execution engine interface.
    /// </summary>
    public interface IActionExecutionEngine
    {
        #region Public Events

        /// <summary>On progress changed.</summary>
        event EventHandler<ProgressChangedArgs> OnProgressChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>Asynchronously executes all actions in the <see cref="ActionPlan"/> that is specified in <paramref name="plan"/>.</summary>
        /// <param name="plan">The <see cref="ActionPlan"/> to execute.</param>
        /// <param name="properties">Collection of global properties. Names should match those returned by <see cref="ActionPlan.Properties"/>.</param>
        /// <param name="token">The cancellation token that can be used for cancelling execution.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task ExecuteAsync(ActionPlan plan, Dictionary<string, object> properties, CancellationToken token);

        /// <summary>Reloads all available action handlers.</summary>
        void ReloadAvailableActions();

        /// <summary>
        /// Validates <see cref="ActionPlan"/>
        /// </summary>
        /// <param name="plan"><see cref="ActionPlan"/> to validate.</param>
        void Validate(ActionPlan plan);

        #endregion
    }
}