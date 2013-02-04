// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="TfsBranchToolVSExtensionPackage.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The ...
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.VSExtension
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;
    using Microsoft.Internal.VisualStudio.PlatformUI;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
  
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// <para></para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// <para></para>
    /// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    /// a package.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidTfsBranchToolVSExtensionPkgString)]
    public sealed class TfsBranchToolVSExtensionPackage : Package
    {
        private TeamExplorerIntegrator teamexpIntegrator;

        /// <summary>
        /// Initializes a new instance of the TfsBranchToolVSExtensionPackage class.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TfsBranchToolVSExtensionPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }        

        public TeamExplorerIntegrator TeamExplorerIntegrator
        {
            get
            {
                if (this.teamexpIntegrator == null)
                {
                    this.teamexpIntegrator = new TeamExplorerIntegrator(
                        this.GetService(typeof(EnvDTE.IVsExtensibility)) as EnvDTE.IVsExtensibility,
                        (ITeamFoundationContextManager)this.GetService(typeof(ITeamFoundationContextManager)));
                }

                return this.teamexpIntegrator;
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.GuidTfsBranchToolVSExtensionCmdSet, (int)PkgCmdIDList.CmdidAlmRangersBranchingTooling);
                MenuCommand menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
             
            try
            {
                uiShell.SetWaitCursor();
             
                switch ((uint)((MenuCommand)sender).CommandID.ID)
                {
                    case PkgCmdIDList.CmdidAlmRangersBranchingTooling:
                        Views.SelectBranchPlan dlg = new Views.SelectBranchPlan(TeamExplorerIntegrator, CatalogFactory.CreateFileSystemCatalog());
                        WindowHelper.ShowModal(dlg);

                        break;
                }                
            }
            catch (Exception ex)
            {
                Guid clsid = Guid.Empty;
                int result;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           "TfsBranchToolVSExtension",
                           string.Format(CultureInfo.CurrentCulture, ex.Message),
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_INFO,
                           0,        // false
                           out result));
            }
        }
    }
}
