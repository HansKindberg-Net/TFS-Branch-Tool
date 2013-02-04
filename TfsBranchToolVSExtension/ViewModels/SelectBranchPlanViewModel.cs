// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectBranchPlanViewModel.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The ViewModel of the BranchPlan
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.VSExtension.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;    
    using System.Windows.Input;
    using System.Windows.Threading;

    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;

    /// <summary>
    /// An enumeration for the JobStatus
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// notStarted
        /// </summary>
        notStarted,

        /// <summary>
        /// inProgress
        /// </summary>        
        inProgress,

        /// <summary>
        /// done
        /// </summary>
        done
    }

    public class BranchPlanArgument
        : ActionPropertyInfo
    {
        public BranchPlanArgument(string argName)
        {
            this.Name = argName;
            this.Value = string.Empty;
        }

        public bool IsHandeledByExtension
        {
            get
            {
                if (this.Name == "ProjectCollectionUrl" || this.Name == "TeamProject")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    
    public class SelectBranchPlanViewModel
        : INotifyPropertyChanged
    {
        #region "Private members"

        private List<BranchPlanArgument> lstArguments;
        private ActionPlan theSelectedBranchPlan;

        private ITeamExplorerIntegrator teamExplorer;
        private IActionExecutionEngine actionEngine;

        private IPlanCatalog planCatalog;
        private double progress;
        private JobStatus status;

        private string operationLog;
        private string currentOperation;

        #endregion        
    
        #region Constructor

        public SelectBranchPlanViewModel()
        {
            JobStatus = JobStatus.notStarted;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region "Public properties"

        public JobStatus JobStatus
        {
            get 
            { 
                return this.status; 
            }

            set 
            { 
                this.status = value; 
                this.NotifyPropertyChanged("JobStatus");
                this.NotifyPropertyChanged("ShowProgress");
                this.NotifyPropertyChanged("ShowForm");
                this.NotifyPropertyChanged("IsJobDone"); 
            }        
        }

        public bool ShowProgress 
        {
            get { return this.JobStatus == JobStatus.inProgress || this.JobStatus == JobStatus.done; }
        }
        
        public bool ShowForm
        {
            get { return this.JobStatus == JobStatus.notStarted; }
        }
        
        public bool IsJobDone
        {
            get { return this.JobStatus == JobStatus.done; }
        }

        public double JobProgress
        {
            get 
            { 
                return this.progress; 
            }

            set 
            { 
                this.progress = value; 
                this.NotifyPropertyChanged("JobProgress"); 
            }
        }

        public string JobCurrentOperation
        {
            get 
            { 
                return this.currentOperation; 
            }

            set 
            { 
                this.currentOperation = value; 
                this.NotifyPropertyChanged("JobCurrentOperation"); 
            }
        }

        public string JobOperationLog
        {
            get 
            { 
                return this.operationLog; 
            }

            set 
            { 
                this.operationLog = value; 
                this.NotifyPropertyChanged("JobOperationLog"); 
            }
        }

        public IEnumerable<ActionPlan> BranchPlans 
        {
            get 
            {
                if (this.planCatalog != null)
                {
                    return this.planCatalog.Plans;
                }
                else
                {
                    return null;
                }
            }                      
        }

        public ActionPlan SelectedBranchPlan 
        {
            get 
            { 
                return this.theSelectedBranchPlan; 
            }

            set
            {
                this.theSelectedBranchPlan = value;
                this.Arguments = null;
                this.NotifyPropertyChanged("Arguments");
                this.NotifyPropertyChanged("SelectedBranchPlan");
            }
        }

        public List<BranchPlanArgument> Arguments
        {
            get
            {
                if (this.lstArguments == null)
                {
                   this.lstArguments = this.LoadArguments();
                }

                return this.lstArguments;
            }

            set
            {
                this.lstArguments = value;
                this.NotifyPropertyChanged("Arguments");
            }
        }

        #endregion

        #region "Public methods"        

        public void Load(ITeamExplorerIntegrator te, IPlanCatalog branchPlanCatalog, IActionExecutionEngine engine)
        {
            this.teamExplorer = te;
            this.planCatalog = branchPlanCatalog;
            this.SelectedBranchPlan = this.BranchPlans.First();
            this.actionEngine = engine;
        }        
         
        public void UpdateStatus(double value, string currentOperation)
        {          
            this.JobProgress = value;
            this.JobCurrentOperation = currentOperation;
            this.JobOperationLog += currentOperation + "\n";

            if (value == 100)
            {
                JobStatus = JobStatus.done;
            }
        }

        public Task ExecuteJob()
        {
            try
            {
                JobStatus = JobStatus.inProgress;

                this.teamExplorer.SetSourceControlExplorerDirty(this.GetBranchPlanArgumentValue("RootFolder"));
                
                this.UpdateStatus(0, "Load engine");

                var cts = new System.Threading.CancellationTokenSource();

                Task taskLoad = new Task(delegate
                {
                    this.actionEngine.OnProgressChanged += this.OnProgressChanged;

                    this.UpdateStatus(0, "Loading branchplan");

                    ////Dispatcher.Invoke(callback, System.Windows.Threading.DispatcherPriority.Background, new object[] { 0, "Loding branchplan" });
                });
                taskLoad.ContinueWith(delegate
                {
                    try
                    {
                        this.UpdateStatus(0, "Start executing branchplan");
                        ////Dispatcher.Invoke(callback, System.Windows.Threading.DispatcherPriority.Background, new object[] { 0, "Start executing branchplan" });
                        Task t = this.actionEngine.ExecuteAsync(this.SelectedBranchPlan, this.Arguments.ToDictionary(x => x.Name, x => x.Value as object), cts.Token);
                        t.Wait();
                        this.teamExplorer.RefreshSourceControlExplorer();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is ArgumentOutOfRangeException)
                        {
                            this.UpdateStatus(100, "\nValidation error: " + ExtractFirstRow(ex.InnerException.Message));
                        }
                        else
                        {
                            this.UpdateStatus(100, "\nExecution error: \n" + ex.InnerException.Message);
                        }
                    }
                });
                taskLoad.Start();
                return taskLoad;
            }
            catch (Exception ex)
            {
                this.UpdateStatus(100, "Error: \n" + ex.Message + "\n" + ex.StackTrace);
            }

            return null;
        }

        #region INotifyPropertyChanged Members

        public void NotifyPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #endregion

        #region Private and Protected methods

        protected List<BranchPlanArgument> LoadArguments()
        {
            List<BranchPlanArgument> d = new List<BranchPlanArgument>();
            if (this.planCatalog != null)
            {
                foreach (var arg in this.SelectedBranchPlan.Properties)
                {
                    d.Add(new BranchPlanArgument(arg.Name) { Optional = arg.Optional, TypeInformation = arg.TypeInformation, Value = arg.Value });
                }
            }
            else
            {
                d.Add(new BranchPlanArgument("ProjectCollectionUrl") { Optional = false, Value = @"http://yourserver.com" });
                d.Add(new BranchPlanArgument("Design mode  2") { Optional = false, Value = "true", TypeInformation = "string" });
                d.Add(new BranchPlanArgument("Design mode  3") { Optional = false, Value = "true", TypeInformation = "bool" });
            }

            if (this.teamExplorer != null)
            {                
                this.SetBranchPlanArgumentValue(d, "ProjectCollectionUrl", this.teamExplorer.TPCollectionUri.ToString());
                this.SetBranchPlanArgumentValue(d, "TeamProject", this.teamExplorer.TPName);
                this.SetBranchPlanArgumentValue(d, "RootFolder", this.teamExplorer.CurrentSourceControlFolder);                
            }

            return d;
        }

        private static string ExtractFirstRow(string s)
        {
            if (s.IndexOf("\n") > 0)
            {
                s = s.Substring(0, s.IndexOf("\n"));
            }

            return s;
        }

        private void OnProgressChanged(object sender, ProgressChangedArgs e)
        {
            this.UpdateStatus(e.Progress, e.Message);
        }

        private void SetBranchPlanArgumentValue(List<BranchPlanArgument> lst, string argName, string value)
        {
            if (lst.FirstOrDefault(x => x.Name == argName) != null)
            {
                lst.First(x => x.Name == argName).Value = value;
            }
        }

        private string GetBranchPlanArgumentValue(string argName)
        {
            if (this.Arguments.FirstOrDefault(x => x.Name == argName) != null)
            {
                return this.Arguments.First(x => x.Name == argName).Value;
            }

            return null;
        }

        #endregion
    }
}
