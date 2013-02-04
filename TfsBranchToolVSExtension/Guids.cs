// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Guids.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Definition of Guids
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// Guids.cs
// MUST match guids.h

namespace Microsoft.ALMRangers.BranchTool.VSExtension
{
    using System;

    internal static class GuidList
    {
        public const string GuidTfsBranchToolVSExtensionPkgString = "16f91d46-d2b8-4bd0-a12f-c2dea4aa2f49";
        public const string GuidTfsBranchToolVSExtensionCmdSetString = "4caa017e-c24f-4c0d-8837-855c4c52da83";

        public static readonly Guid GuidTfsBranchToolVSExtensionCmdSet = new Guid(GuidTfsBranchToolVSExtensionCmdSetString);
    }
}