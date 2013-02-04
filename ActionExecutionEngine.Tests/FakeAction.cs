// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeAction.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Fake action for testing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    /// <summary>
    /// The create folder.
    /// </summary>
    [Export(typeof(IAction))]
    [ExportMetadata("Name", "FakeAction")]
    [ExportMetadata("Version", 1)]
    public class FakeAction : IAction
    {
        #region Public Properties

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public string PropertyNoSetter { get; private set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public string PropertyNoGetter { private get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public bool ResultProperty 
        { 
            get { return true; } 
        }
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Global
        #endregion

        #region Public Methods and Operators

        /// <summary>The execute async.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/></returns>
        public Task ExecuteAsync(IActionExecutionContext context)
        {
            var task = new Task(() => this.PropertyNoSetter = "FakeActionResult");

            task.Start();

            return task;
        }

        #endregion
    }
}