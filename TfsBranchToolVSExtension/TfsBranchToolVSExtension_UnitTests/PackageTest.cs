// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageTest.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the PackageTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.VSExtension_UnitTests
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;
    using Microsoft.ALMRangers.BranchTool.VSExtension;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VsSDK.UnitTestLibrary;
    
    [TestClass]
    public class PackageTest
    {
        [TestMethod]
        public void CreateInstance()
        {
            TfsBranchToolVSExtensionPackage package = new TfsBranchToolVSExtensionPackage();
        }

        [TestMethod]
        public void IsIVsPackage()
        {
            TfsBranchToolVSExtensionPackage package = new TfsBranchToolVSExtensionPackage();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
        }

        [TestMethod]
        public void SetSite()
        {
            // Create the package
            IVsPackage package = new TfsBranchToolVSExtensionPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}
