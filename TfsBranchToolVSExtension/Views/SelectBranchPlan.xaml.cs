// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="SelectBranchPlan.xaml.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The packaged commands for the VSExtension
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.VSExtension.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;

    /// <summary>
    /// Interaction logic for SelectBranchPlan.xaml
    /// </summary>
    public partial class SelectBranchPlan : Window
    {
        private ViewModels.SelectBranchPlanViewModel vm;

        public SelectBranchPlan()
        {
            this.InitializeComponent();
        }

        public SelectBranchPlan(TeamExplorerIntegrator te, IPlanCatalog branchPlanCatalog)
        {
            this.InitializeComponent();

            this.vm = new ViewModels.SelectBranchPlanViewModel();
            this.vm.Load(te, branchPlanCatalog, ActionExecutionEngineFactory.CreateActionExecutionEngine());
            this.DataContext = this.vm;
        }

        private void CmdApply_Click(object sender, RoutedEventArgs e)
        {
            this.vm.ExecuteJob();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Browser != null)
            {
                try
                {
                    this.Browser.Navigate(this.vm.SelectedBranchPlan.HelpUri);
                }
                catch { }
            }
        }

        private void CmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
