// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="PkgCmdID.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The packaged commands for the VSExtension
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// PkgCmdID.cs
// MUST match PkgCmdID.h
namespace Microsoft.ALMRangers.BranchTool.VSExtension
{
    internal static class PkgCmdIDList
    {
        public const uint MenuidSubMenu = 0x0104;
        public const uint GrpBranchToolMainMenu = 0x1100;
        public const uint GrpSCEMainMenu = 0x1101;
        public const uint CmdidAlmRangersBranchingTooling = 0x100;
    }
}