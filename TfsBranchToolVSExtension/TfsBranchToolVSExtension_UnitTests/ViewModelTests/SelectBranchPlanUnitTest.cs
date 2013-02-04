// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectBranchPlanUnitTest.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Unit test for the select branchplan viewmodel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsBranchToolVSExtension_UnitTests.ViewModelTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;
    using Microsoft.ALMRangers.BranchTool.VSExtension;
    using Microsoft.ALMRangers.BranchTool.VSExtension.ViewModels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class SelectBranchPlanViewModelUnitTest
    {
        [TestMethod]
        public void ViewModel_ValidateVisiblFlags()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            vm.Load(mockTE, mockPlanCataog, null);

            bool showFormStart = vm.ShowForm;
            bool showProgressStart = vm.ShowProgress;
            bool showDoneStart = vm.IsJobDone;
            Assert.AreEqual(showFormStart, true);
            Assert.AreEqual(showProgressStart, false);
            Assert.AreEqual(showDoneStart, false);

            vm.JobStatus = JobStatus.inProgress;

            bool showFormInProgress = vm.ShowForm;
            bool showProgressInProgress = vm.ShowProgress;
            bool showDoneInPogress = vm.IsJobDone;
            Assert.AreEqual(showFormInProgress, false);
            Assert.AreEqual(showProgressInProgress, true);
            Assert.AreEqual(showDoneInPogress, false);

            vm.JobStatus = JobStatus.done;

            bool showFormDone = vm.ShowForm;
            bool showProgressDone = vm.ShowProgress;
            bool showDoneDone = vm.IsJobDone;
            Assert.AreEqual(showFormDone, false);
            Assert.AreEqual(showProgressDone, true);
            Assert.AreEqual(showDoneDone, true);
        }

        [TestMethod]
        public void ViewModel_ValidateProgress()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            vm.Load(mockTE, mockPlanCataog, null);

            string started = "Started";
            vm.UpdateStatus(0, started);
            
            Assert.AreEqual(vm.JobProgress, 0);
            Assert.AreEqual(vm.JobCurrentOperation, started);
            Assert.AreEqual(vm.JobOperationLog, started + "\n");

            string op1 = "Operation1";
            vm.UpdateStatus(10, op1);

            Assert.AreEqual(vm.JobProgress, 10);
            Assert.AreEqual(vm.JobCurrentOperation, op1);
            Assert.AreEqual(vm.JobOperationLog, started + "\n" + op1 + "\n");

            string op2 = "Operation2";
            vm.UpdateStatus(100, op2);
            Assert.AreEqual(vm.JobProgress, 100);
            Assert.AreEqual(vm.IsJobDone, true);
            Assert.AreEqual(vm.JobCurrentOperation, op2);
            Assert.AreEqual(vm.JobOperationLog, started + "\n" + op1 + "\n" + op2 + "\n");
        }

        [TestMethod]
        public void ViewModel_ValidateExecute()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockEngine engine = new MockEngine();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            vm.Load(mockTE, mockPlanCataog, engine);

            vm.ExecuteJob();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ViewModel_ValidateExecuteErrorLoading()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockEngine engine = new MockEngine();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            vm.Load(mockTE, mockPlanCataog, null);

            Task t = vm.ExecuteJob();
            t.Wait();

            Assert.AreEqual(vm.JobProgress, 100.0); 
            Assert.AreEqual(vm.JobCurrentOperation, "Error: \nObject reference not set to an instance of an object.\n   at Microsoft.ALMRangers.BranchTool.VSExtension.ViewModels.SelectBranchPlanViewModel.<>c__DisplayClass5.<ExecuteJob>b__1(Task param0) in c:\\Src_Rangers\\BranchTool\\code\\TfsBranchToolVSExtension\\ViewModels\\SelectBranchPlanViewModel.cs:line 268");
        }

        [TestMethod]
        public void ViewModel_LoadTeamExplorerTestBasic()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            Dictionary<string, object> d;
            vm.Load(mockTE, mockPlanCataog, null);
            vm.SelectedBranchPlan = mockPlanCataog.GetPlan("Basic");
            d = vm.Arguments.ToDictionary(x => x.Name, x => x.Value as object);

            object actualCollection, actualTPName, actualRoot;

            d.TryGetValue("ProjectCollectionUrl", out actualCollection);
            d.TryGetValue("TeamProject", out actualTPName);
            d.TryGetValue("RootFolder", out actualRoot);
            
            Assert.AreEqual(actualCollection.ToString(), mockTE.TPCollectionUri.ToString());
            Assert.AreEqual(actualTPName.ToString(), mockTE.TPName);
            Assert.AreEqual(actualRoot.ToString(), mockTE.CurrentSourceControlFolder);
        }

        [TestMethod]
        public void ViewModel_NotLoadedTest_Argument()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            List<BranchPlanArgument> lst;

            lst = vm.Arguments;

            Assert.AreEqual(lst.Count, 3);
        }

        [TestMethod]
        public void ViewModel_NotLoadedTest_BranchPlans()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer mockTE = new MockTeamExplorer();
            MockPlanCatalog mockPlanCataog = new MockPlanCatalog();
            IEnumerable<ActionPlan> lst;

            lst = vm.BranchPlans;

            Assert.AreEqual(lst, null);
        }

        [TestMethod]
        public void ViewModel_BranchPlanArgument()
        {
            BranchPlanArgument branchplanArg = new BranchPlanArgument("ProjectCollectionUrl");
            Assert.AreEqual(branchplanArg.IsHandeledByExtension, true);

            BranchPlanArgument branchplanArgTeamProject = new BranchPlanArgument("TeamProject");
            Assert.AreEqual(branchplanArgTeamProject.IsHandeledByExtension, true);

            BranchPlanArgument branchplanArgDummy = new BranchPlanArgument("Dummy");
            branchplanArgDummy.Description = "This is a description of the argument";
            branchplanArgDummy.Value = "DefaultValue";
            branchplanArgDummy.Optional = false;

            Assert.AreEqual(branchplanArgDummy.IsHandeledByExtension, false);
            Assert.AreEqual(branchplanArgDummy.Value, "DefaultValue");
            Assert.AreEqual(branchplanArgDummy.Description, "This is a description of the argument");
            Assert.AreEqual(branchplanArgDummy.Optional, false);
        }

        [TestMethod]
        public void ViewModel_BranchPlanTest()
        {
            SelectBranchPlanViewModel vm = new SelectBranchPlanViewModel();
            MockTeamExplorer te = new MockTeamExplorer();
            MockPlanCatalog pc = new MockPlanCatalog();
            vm.Load(te, pc, null);

            IEnumerable<ActionPlan> lst2 = vm.BranchPlans;
            List<ActionPlan> lst = new List<ActionPlan>();
            lst.AddRange(lst2);

            Assert.AreEqual(lst.Count, 2);

            Assert.AreEqual(lst[0].Name, "Basic");
            Assert.AreEqual(lst[1].Name, "Feature");
        }
    }
    
    public class MockTeamExplorer
        : ITeamExplorerIntegrator
    {
        private string name = "TestProjekt";

        public Uri TPCollectionUri 
        { 
            get { return new Uri(@"http://TestServer.local:8080/tfs/DefaultCollection"); } 
        }

        public string CurrentSourceControlFolder 
        { 
            get { return @"$/TestProjekt/ProductA"; } 
        }

        public string TPName 
        { 
            get { return this.name; } 
            
            set { this.name = value; } 
        }

        public void SetSourceControlExplorerDirty(string serverpath)
        {
        }
        public void RefreshSourceControlExplorer()
        {
        }        

    }

    public class MockEngine
        : IActionExecutionEngine
    {
        #region Public Events

        /// <summary>On progress changed event.</summary>
        public event EventHandler<ProgressChangedArgs> OnProgressChanged;
        
        #endregion

        #region Public Properties

        /// <summary>Gets the global property names.</summary>
        public string[] GlobalPropertyNames 
        {
            get
            {
                return new string[] { "COLLECTION", "TeamProjectName", "BRANCHROOT", "FeatureName" };
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>This method executed actions previously loaded by <see cref="LoadActionsDefinition"/> method.</summary>
        /// <param name="plan">the plan  </param>
        /// <param name="properties">Collection of global properties. Names should match those returned by <see cref="GlobalPropertyNames"/>.</param>
        /// <param name="token">The cancellation token that can be used for cancelling execution.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task ExecuteAsync(ActionPlan plan, Dictionary<string, object> properties, CancellationToken token)
        {
            return null;
        }

        /// <summary>The load actions definition.</summary>
        /// <param name="definitionFileName">The definition file name.</param>
        public void LoadActionsDefinition(string definitionFileName)
        {
        }

        /// <summary>Reload all available actions.</summary>
        public void ReloadAvailableActions()
        {
        }

        /// <summary>
        /// Validates <see cref="ActionPlan"/>
        /// </summary>
        /// <param name="plan"><see cref="ActionPlan"/> to validate.</param>
        public void Validate(ActionPlan plan)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MockPlanCatalog
        : IPlanCatalog
    {
        private List<ActionPlan> lstPlans;

        public IEnumerable<ActionPlan> Plans
        {
            get
            {
                this.lstPlans = new List<ActionPlan>
                    {
                        new ActionPlan
                            {
                                Name = "Basic",
                                HelpUri = new Uri(
                                    "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "/basic.html"),
                                Properties =
                                    new[]
                                        {
                                            new ActionPropertyInfo { Name = "ProjectCollectionUrl" },
                                            new ActionPropertyInfo { Name = "TeamProject" },
                                            new ActionPropertyInfo { Name = "RootFolder", Optional = true, Value = "Dev" }
                                        }
                            },
                        new ActionPlan
                            {
                                Name = "Feature",
                                HelpUri = new Uri(
                                    "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "/Feature.html"),
                                Properties =
                                    new[]
                                        {
                                            new ActionPropertyInfo { Name = "ProjectCollectionUrl" },
                                            new ActionPropertyInfo { Name = "TeamProject" },
                                            new ActionPropertyInfo { Name = "RootFolder", Optional = true, Value = "Dev" },
                                            new ActionPropertyInfo { Name = "FeatureName" }
                                        }
                            }
                    };
                
                return this.lstPlans;
            }
        }

        public ActionPlan GetPlan(string name)
        {
            return this.lstPlans.First(x => x.Name == name);
        }
    }
}
