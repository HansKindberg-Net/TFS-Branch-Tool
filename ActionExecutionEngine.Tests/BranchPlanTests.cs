// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BranchPlanTests.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the BranchPlanTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem("Branch Plans", "Branch Plans")]
    public class BranchPlanTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BranchPlanFileSystemCatalogConstructorParametersNullTest()
        {
// ReSharper disable ObjectCreationAsStatement
            new FileSystemCatalog(null);
// ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BranchPlanFileSystemCatalogConstructorParametersWrongPathTest()
        {
// ReSharper disable ObjectCreationAsStatement
            new FileSystemCatalog(Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString()));
// ReSharper restore ObjectCreationAsStatement
        }

        [TestMethod]
        public void BranchPlanCatalogCreateFileSystemCatalogTest()
        {
            var catalog = CatalogFactory.CreateFileSystemCatalog();

            Assert.IsNotNull(catalog);
            Assert.IsNotNull(catalog.GetPlan("Basic"));
        }

        [TestMethod]
        public void BranchPlanCatalogPropertiesTest()
        {
            var catalog = CatalogFactory.CreateFileSystemCatalog();
            Assert.IsNotNull(catalog);

            var plan = catalog.GetPlan("Basic");

            Assert.IsNotNull(plan);

            var properties = plan.Properties;

            Assert.IsNotNull(properties);
            
            Assert.AreEqual(2, properties.Length);
            Assert.IsNotNull(properties.FirstOrDefault(info => info.Name == "ProjectCollectionUrl" && !info.Optional));
            Assert.IsNotNull(properties.FirstOrDefault(info => info.Name == "RootFolder" && !info.Optional));
        }
    }
}