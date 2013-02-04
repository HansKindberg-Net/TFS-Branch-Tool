// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CommandLineParser.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The action execution engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.CmdLineParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommandLineParser
    {
        private BranchPlanBaseCommand cmd;

        public CommandLineParser()
        {
            this.cmd = new BranchPlanBaseCommand();
        }

        public CommandLineParser(BranchPlanBaseCommand cmdInput)
        {
            this.cmd = cmdInput;
        }

        public int TestableMain(string[] args)
        {
            int retVal;

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!args.Any() || args[0] == @"/?" || args[0] == "help" || args[0] == "/help")
            {                                
                retVal = this.cmd.ShowUsage();
                return retVal;
            }

            retVal = this.cmd.ValidateOperation(args[0]);
            if (retVal != 0)
            {
                return retVal;
            }            
            
            if (args.Length == 1 || args[1] == @"/?" || args[1] == "help" || args[1] == "/help")
            {                
                retVal = this.cmd.ShowValidUsage(args[0]);
                return retVal;
            }            
            
            if (retVal == 0)
            {                
                List<string> lstArg = new List<string>();
                lstArg.AddRange(args);
                lstArg.RemoveAt(0);
                retVal = this.cmd.ParseArguments(lstArg.ToArray());
                if (retVal == 0)
                {
                    retVal = this.cmd.Validate();
                    if (retVal == 0)
                    {
                        retVal = this.cmd.Run();
                    }
                }                                       
            }

            return retVal;
        }  
    }
}
