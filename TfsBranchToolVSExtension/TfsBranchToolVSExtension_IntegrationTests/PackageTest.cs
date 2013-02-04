// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageTest.cs" company="Microsoft Corporation">
//   Copyright © Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Integration test for package validation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TfsBranchToolVSExtension_IntegrationTests
{
    using System;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VSSDK.Tools.VsIdeTesting;

    /// <summary>
    /// Integration test for package validation
    /// </summary>
    [TestClass]
    public class PackageTest
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

        [TestMethod]
        [HostType("VS IDE")]
        [Ignore]
        public void VSExtension_PackageLoadTest()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate
            {
                // Get the Shell Service
                IVsShell shellService = VsIdeTestHostContext.ServiceProvider.GetService(typeof(SVsShell)) as IVsShell;
                Assert.IsNotNull(shellService);

                // Validate package load
                IVsPackage package;
                Guid packageGuid = new Guid(Microsoft.ALMRangers.BranchTool.VSExtension.GuidList.GuidTfsBranchToolVSExtensionPkgString);
                Assert.IsTrue(0 == shellService.LoadPackage(ref packageGuid, out package));
                Assert.IsNotNull(package, "Package failed to load");
            });
        }
    }
}
