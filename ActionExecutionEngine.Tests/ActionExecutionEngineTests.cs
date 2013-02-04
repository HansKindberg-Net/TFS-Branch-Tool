// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ActionExecutionEngineTests.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The rules engine tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The rules engine tests.
    /// </summary>
    [TestClass]
    public class ActionExecutionEngineTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The rules engine constructor test.
        /// </summary>
        [TestMethod]
        public void ActionExecutionEngineConstructorTest()
        {
            Assert.AreNotEqual(null, ActionExecutionEngineFactory.CreateActionExecutionEngine());
        }

        /// <summary>
        /// The rules engine dispose test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ActionExecutionEngineDisposeTest()
        {
            var rulesEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();

            ((IDisposable)rulesEngine).Dispose();
            rulesEngine.ReloadAvailableActions();
        }

        /// <summary>
        /// The rules engine load wrong rule action test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("fakeactionpropertytest1.xml")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineLoadActionWithWrongInPropertyTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("fakeactionpropertytest1.xml"));
        }

        /// <summary>The action execution engine load action with wrong out property test.</summary>
        [TestMethod]
        [DeploymentItem("fakeactionpropertytest2.xml")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineLoadActionWithWrongOutPropertyTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("fakeactionpropertytest2.xml"));
        }

        /// <summary>The action execution engine load action with missing in property test.</summary>
        [TestMethod]
        [DeploymentItem("fakeactionpropertytest3.xml")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineLoadActionWithMissingInPropertyTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("fakeactionpropertytest3.xml"));
        }

        /// <summary>The action execution engine load action with missing out property test.</summary>
        [TestMethod]
        [DeploymentItem("fakeactionpropertytest4.xml")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineLoadActionWithMissingOutPropertyTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("fakeactionpropertytest4.xml"));
        }

        /// <summary>
        /// Load actions definition test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("sampleactionplan.xml")]
        public void ActionExecutionEngineLoadActionsDefinitionTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("sampleactionplan.xml"));
        }
        
        /// <summary>
        /// Non-mandatory global property with no default value test
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [DeploymentItem("wrongoptonalparam.xml")]
        public void ActionExecutionEngineLoadWrongGlopalPropertyTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("wrongoptonalparam.xml"));
        }

        [TestMethod]
        [DeploymentItem("fakeactionset.xml")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ActionExecutionEngineExecuteGlobalPropertiesArgumentNullTest()
        {
            var actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();
            
            var cts = new CancellationTokenSource();

            await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("fakeactionset.xml"), null, cts.Token);
        }

        [TestMethod]
        [DeploymentItem("ValidateOutputPropertyNoValidationError.xml")]
        public async Task ActionExecutionEngineExecuteValidationOKTest()
        {
            var actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();

            var globalProperties = new Dictionary<string, object>
                {
                    { "ProjectCollectionUrl", "http://tfs.fakes.com:8080/tfs/DefaultCollection" },
                    { "RootFolder", "$/MyRoot" },
                    { "DevBranchName", "MyDevBranch" }
                };

            var cts = new CancellationTokenSource();

            await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("ValidateOutputPropertyNoValidationError.xml"), globalProperties, cts.Token);
        }

        [TestMethod]
        [DeploymentItem("ValidateOutputPropertyNoValidationMessage.xml")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task ActionExecutionEngineExecuteValidationWithoutCustomValidationMessage()
        {
            var actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();

            var globalProperties = new Dictionary<string, object>
                {
                    { "ProjectCollectionUrl", "http://tfs.fakes.com:8080/tfs/DefaultCollection" },
                    { "RootFolder", "$/MyRoot" },
                    { "DevBranchName", "MyDevBranch" }
                };

            var cts = new CancellationTokenSource();

            await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("ValidateOutputPropertyNoValidationMessage.xml"), globalProperties, cts.Token);
        }

        [TestMethod]
        [DeploymentItem("ValidateOutputPropertyWithValidationMessage.xml")]
        public async Task ActionExecutionEngineExecuteValidationFailWithCustomMessageTest()
        {
            var actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();

            var globalProperties = new Dictionary<string, object>
                {
                    { "ProjectCollectionUrl", "http://tfs.fakes.com:8080/tfs/DefaultCollection" },
                    { "RootFolder", "$/MyRoot" },
                    { "DevBranchName", "MyDevBranch" }
                };

            var cts = new CancellationTokenSource();

            try
            {
                await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("ValidateOutputPropertyWithValidationMessage.xml"), globalProperties, cts.Token);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.StartsWith("Custom validation message with expansion $/MyRoot/FOO already exist"));
            }
        }

        [TestMethod]
        [DeploymentItem("fakeactionset.xml")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task ActionExecutionEngineExecuteGlobalPropertiesArgumentTest()
        {
            var actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();
            
            var globalProperties = new Dictionary<string, object>
                {
                    { "ProjectCollectionUrl", "http://tfs.fakes.com:8080/tfs/DefaultCollection" },
                    { "rrRootFolder", "$/MyRoot" },
                    { "DdDevBranchName", "MyDevBranch" }
                };

            var cts = new CancellationTokenSource();

            await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("fakeactionset.xml"), globalProperties, cts.Token);
        }

        [TestMethod]
        [DeploymentItem("fakeactionset.xml")]
        public async Task ActionExecutionEngineExecuteTest()
        {
            IActionExecutionEngine actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();

            var globalProperties = new Dictionary<string, object>
                {
                    { "ProjectCollectionUrl", "http://tfs.fakes.com:8080/tfs/DefaultCollection" },
                    { "RootFolder", "$/MyRoot" }
                };

            var cts = new CancellationTokenSource();

            await actionExecutionEngine.ExecuteAsync(ActionPlan.Load("fakeactionset.xml"), globalProperties, cts.Token);
        }

        /// <summary>
        /// The rules engine load rules definition wrong argument test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineValidateWrongArgumentTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(null);
        }

        /// <summary>
        /// The rules engine load wrong rule action test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("wrongactionname.xml")]
        [ExpectedException(typeof(InvalidDataException), AllowDerivedTypes = true)]
        public void ActionExecutionEngineValidateWrongActionTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().Validate(ActionPlan.Load("wrongactionname.xml"));
        }

        /// <summary>The action execution engine reload available actions test.</summary>
        [TestMethod]
        public void ActionExecutionEngineReloadAvailableActionsTest()
        {
            ActionExecutionEngineFactory.CreateActionExecutionEngine().ReloadAvailableActions();
        }

        #endregion
    }
}