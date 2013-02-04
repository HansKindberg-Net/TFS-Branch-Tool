// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemCatalog.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the FileSystemCatalog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents branch plans catalog in file system
    /// </summary>
    public class FileSystemCatalog : IPlanCatalog
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FileSystemCatalog"/> class.</summary>
        /// <param name="path">Path to the directory where plan definitions should be populated from.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="path"/> is null or empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The directory represented by <paramref name="path"/> doesn't exist.</exception>
        public FileSystemCatalog(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentOutOfRangeException("path", path, "Specified directory doesn't exist.");
            }

            string schemaFile = Path.Combine(path, "ActionPlan.xsd");
            this.Plans =
                Directory.EnumerateFiles(path, "*.branchplan", SearchOption.AllDirectories).Select(
                    file => ActionPlan.Load(file, schemaFile)).ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets all plans in catalog.
        /// </summary>
        public IEnumerable<ActionPlan> Plans { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Gets specific plan.</summary>
        /// <param name="name">Plan name.</param>
        /// <returns>The <see cref="ActionPlan"/>.</returns>
        public ActionPlan GetPlan(string name)
        {
            return this.Plans.FirstOrDefault(planInfo => planInfo.Name == name);
        }

        #endregion
    }
}