// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemCallback.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the MenuItemTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsBranchToolVSExtension_UnitTests.MenuItemTests
{
    using System.ComponentModel.Design;
    using System.Reflection;
    using Microsoft.ALMRangers.BranchTool.VSExtension;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VsSDK.UnitTestLibrary;

    [TestClass]
    public class MenuItemTest
    {
        /// <summary>
        /// Verify that a new menu command object gets added to the OleMenuCommandService. 
        /// This action takes place In the Initialize method of the Package object
        /// </summary>
        [TestMethod]
        public void InitializeMenuCommand()
        {
            // Create the package
            IVsPackage package = new TfsBranchToolVSExtensionPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Verify that the menu command can be found
            CommandID menuCommandID = new CommandID(Microsoft.ALMRangers.BranchTool.VSExtension.GuidList.GuidTfsBranchToolVSExtensionCmdSet, (int)Microsoft.ALMRangers.BranchTool.VSExtension.PkgCmdIDList.CmdidAlmRangersBranchingTooling);
            System.Reflection.MethodInfo info = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info);
            OleMenuCommandService mcs = info.Invoke(package, new object[] { typeof(IMenuCommandService) }) as OleMenuCommandService;
            Assert.IsNotNull(mcs.FindCommand(menuCommandID));
        }

        [TestMethod]
        public void MenuItemCallback()
        {
            // Create the package
            IVsPackage package = new TfsBranchToolVSExtensionPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Create a UIShell service mock and proffer the service so that it can called from the MenuItemCallback method
            BaseMock uishellMock = UIShellServiceMock.GetUiShellInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Invoke private method on package class and observe that the method does not throw
            System.Reflection.MethodInfo info = package.GetType().GetMethod("MenuItemCallback", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, "Failed to get the private method MenuItemCallback throug refplection");
            info.Invoke(package, new object[] { null, null });

            // Clean up services
            serviceProvider.RemoveService(typeof(SVsUIShell));
        }
    }
}
