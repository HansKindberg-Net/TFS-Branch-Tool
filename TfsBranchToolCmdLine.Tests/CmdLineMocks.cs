// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CmdLineMocks.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Mock objects for the CmdLineTests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsBranchToolCmdLine.Tests
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
    
    internal class MockExecutionEngine : IActionExecutionEngine
    {        
        public event EventHandler<ProgressChangedArgs> OnProgressChanged;

        public string[] GlobalPropertyNames
        {
            get
            {
                return this.GlobalPropertyNames;
            }

            set
            {
                this.GlobalPropertyNames = value;
            }
        }

        public void SetGlobalPropertyNames(string[] values)
        {
            this.GlobalPropertyNames = values;
        }

        public Task ExecuteAsync(ActionPlan plan, Dictionary<string, object> properties, CancellationToken token)
        {        
            return null;            
        }

        /// <summary>The load actions definition.</summary>
        /// <param name="definitionFileName">The definition file name.</param>
        public void LoadActionsDefinition(string definitionFileName)
        {
        }

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

        private void MyAction()
        {
            return;
        }        
    }

    internal class MockPlanCatalog : IPlanCatalog
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
                                Name = "BASIC",
                                HelpUri = new Uri(
                                    "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "/basic.html"),
                                Properties =
                                    new[]
                                        {
                                            new ActionPropertyInfo { Name = "ProjectCollectionUrl" },
                                            new ActionPropertyInfo { Name = "RootFolder" }
                                        }
                            },
                        new ActionPlan
                            {
                                Name = "FEATURE",
                                HelpUri = new Uri(
                                    "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "/Feature.html"),
                                Properties =
                                    new[]
                                        {
                                            new ActionPropertyInfo { Name = "ProjectCollectionUrl" },
                                            new ActionPropertyInfo { Name = "RootFolder" },
                                            new ActionPropertyInfo { Name = "FeatureName" }
                                        }
                            },
                            new ActionPlan
                            {
                                Name = "BOOLTEST",
                                HelpUri = new Uri(
                                    "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "/Feature.html"),
                                Properties =
                                    new[]
                                        {
                                            new ActionPropertyInfo { Name = "ProjectCollectionUrl" },
                                            new ActionPropertyInfo { Name = "BoolTest", TypeInformation = "bool" },
                                            new ActionPropertyInfo { Name = "RootFolder", Optional = true, Value = "Dev" }
                                        }
                            }
                    };

                return this.lstPlans;
            }
        }

        public ActionPlan GetPlan(string name)
        {
            string versalName = name.ToUpper();
            return this.lstPlans.First(x => x.Name == versalName);
        }        
    }
}