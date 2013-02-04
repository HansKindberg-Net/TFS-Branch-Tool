// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IAction.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The IAction interface.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System.Threading.Tasks;

    /// <summary>
    /// The IAction interface.
    /// </summary>
    public interface IAction
    {
        #region Public Methods and Operators

        /// <summary>Asynchronously performs action's job.</summary>
        /// <param name="context">The <see cref="IActionExecutionContext"/> that represents execution context.</param>
        /// <returns><see cref="Task"/></returns>
        Task ExecuteAsync(IActionExecutionContext context);

        #endregion
    }
}