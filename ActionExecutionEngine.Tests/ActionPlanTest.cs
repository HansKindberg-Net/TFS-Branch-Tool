// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionPlanTest.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the ActionPlanTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionPlanTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Action plan load no actions test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("actionplanloadernoaction.xml")]
        [DeploymentItem("actionplanloadernoactions.xml")]
        [DeploymentItem(@"Branch Plans\actionplan.xsd")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ActionPlanLoadNoActionsTest()
        {
            var schemaUri = Path.Combine(this.TestContext.DeploymentDirectory, "actionplan.xsd");
            ActionPlan.Load(Path.Combine(this.TestContext.DeploymentDirectory, "actionplanloadernoaction.xml"), schemaUri);
            ActionPlan.Load(Path.Combine(this.TestContext.DeploymentDirectory, "actionplanloadernoactions.xml"), schemaUri);
        }

        /// <summary>
        /// Action plan load no value for optional property test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("actionplanloadernopropertyvalue.xml")]
        [DeploymentItem(@"Branch Plans\actionplan.xsd")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionPlanLoadNoPropertyValueTest()
        {
            var schemaUri = Path.Combine(this.TestContext.DeploymentDirectory, "actionplan.xsd");
            var fileName = Path.Combine(this.TestContext.DeploymentDirectory, "actionplanloadernopropertyvalue.xml");
            ActionPlan.Load(fileName, schemaUri);
        }

        /// <summary>
        /// Action plan load no actions test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("sampleactionplan.xml")]
        public void ActionPlanLoadNoSchemaValidationTest()
        {
            var fileName = Path.Combine(this.TestContext.DeploymentDirectory, "sampleactionplan.xml");
            Assert.IsNotNull(ActionPlan.Load(fileName));
        }

        /// <summary>
        /// Test that ActionPlan properties are not null.
        /// </summary>
        [TestMethod]
        public void ActionPlanPropertiesNotNullTest()
        {
            var plan = new ActionPlan();

            Assert.IsNotNull(plan.Name);
            Assert.IsNotNull(plan.Description);
            Assert.IsNotNull(plan.Properties);
            Assert.IsNotNull(plan.Actions);
        }

        /// <summary>
        /// Test that ActionInfo properties are not null.
        /// </summary>
        [TestMethod]
        public void ActionInfoPropertiesNotNullTest()
        {
            var actionInfo = new ActionInfo();

            Assert.IsNotNull(actionInfo.Name);
            Assert.IsNotNull(actionInfo.Id);
            Assert.IsNotNull(actionInfo.InputProperties);
            Assert.IsNotNull(actionInfo.OutputProperties);
        }

        /// <summary>
        /// Test that ActionPropertyInfo properties are not null.
        /// </summary>
        [TestMethod]
        public void ActionPropertyInfoPropertiesNotNullTest()
        {
            var propertyInfo = new ActionPropertyInfo();

            Assert.IsNotNull(propertyInfo.Name);
            Assert.IsNotNull(propertyInfo.Description);
            Assert.IsNotNull(propertyInfo.TypeInformation);
        }
    }
}