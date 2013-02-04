// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogFactory.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The catalog factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>Class that creates instances of branch plan catalog.</summary>
    public static class CatalogFactory
    {
        #region Public Methods and Operators

        /// <summary>Creates file system catalog.</summary>
        /// <returns><see cref="IPlanCatalog"/> that represents instance of <see cref="FileSystemCatalog"/>.</returns>
        public static IPlanCatalog CreateFileSystemCatalog()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            if (string.IsNullOrEmpty(directoryName))
            {
                throw new InvalidOperationException("Can't get execution assembly's folder.");
            }

            return new FileSystemCatalog(Path.Combine(directoryName, "Branch Plans"));
        }

        #endregion
    }
}