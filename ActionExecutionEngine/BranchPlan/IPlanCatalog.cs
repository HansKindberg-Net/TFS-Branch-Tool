// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlanCatalog.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the IPlanCatalog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan
{
    using System.Collections.Generic;

    /// <summary>
    /// The IPlanCatalog interface.
    /// </summary>
    public interface IPlanCatalog
    {
        #region Public Properties

        /// <summary>
        /// Gets all plans in catalog.
        /// </summary>
        IEnumerable<ActionPlan> Plans { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets specific plan.</summary>
        /// <param name="name">Plan name.</param>
        /// <returns>The <see cref="ActionPlan"/>.</returns>
        ActionPlan GetPlan(string name);

        #endregion
    }
}