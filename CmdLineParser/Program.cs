// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Program.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The action execution engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.CmdLineParser
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            CommandLineParser cmdParser = new CommandLineParser();
            return cmdParser.TestableMain(args);
        }
    }
}
