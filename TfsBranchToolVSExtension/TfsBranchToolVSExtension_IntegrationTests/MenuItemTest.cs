// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemTest.cs" company="Microsoft Corporation">
//   Copyright © Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the MenuItemTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TfsBranchToolVSExtension_IntegrationTests
{
    using System.ComponentModel.Design;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VsSDK.IntegrationTestLibrary;
    using Microsoft.VSSDK.Tools.VsIdeTesting;

    [TestClass]
    public class MenuItemTest
    {
        private TestContext testContextInstance;

        private delegate void ThreadInvoker();

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        /// <summary>
        /// A test for lauching the command and closing the associated dialogbox
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        [Ignore]
        public void VSExtension_LaunchCommand()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate
            {
                CommandID menuItemCmd = new CommandID(Microsoft.ALMRangers.BranchTool.VSExtension.GuidList.GuidTfsBranchToolVSExtensionCmdSet, (int)Microsoft.ALMRangers.BranchTool.VSExtension.PkgCmdIDList.CmdidAlmRangersBranchingTooling);

                // Create the DialogBoxListener Thread.
                string expectedDialogBoxText = string.Format(CultureInfo.CurrentCulture, "{0}\n\nInside {1}.MenuItemCallback()", "TfsBranchToolVSExtension", "Microsoft.ALMRangers.BranchTool.VSExtension.TfsBranchToolVSExtensionPackage");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    TestUtils testUtils = new TestUtils();
                    testUtils.ExecuteCommand(menuItemCmd);
                }
                finally
                {
                    // Assert.IsTrue(true, "The command executed");
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The dialog box has not shown");
                }
            });
        }
    }
}