// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionExecutionContextTest.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the ActionExecutionContextTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionExecutionContextTest
    {
        [TestMethod]
        public void ActionExecutionContextConstructorCollectionUrlTest()
        {
// ReSharper disable ObjectCreationAsStatement
            new ActionExecutionContext();
// ReSharper restore ObjectCreationAsStatement
        }
    }
}