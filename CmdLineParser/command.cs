// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="command.cs">
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
    using System.Threading.Tasks;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;

    public class CommandBase
    {
        protected const int RETSUCCESS = 0;
        protected const int RETSHOWEDHELP = 1;
        protected const int ERRORUNEXPECTEDARGUMENT = 10;
        protected const int ERRORMISSINGARGUMENT = 11;
        protected const int ERROREMPTYARGUMENT = 12;
        protected const int ERRORARGUMENTWITHOUTSEMICOLON = 13;
        protected const int ERRORDOUBLEARGUMENT = 14;
        protected const int ERRORARGUMENTNOTBOOL = 15;
        protected const int ERRORWRONGOPERATION = 20;
        protected const int ERROROPERATIONNOTFOUND = 30;

        protected int retVal;

        protected List<ActionPropertyInfo> expectedArguments;
        protected List<ActionPropertyInfo> optionalArguments;
        protected List<ActionPlan> validOperations;

        protected Dictionary<string, object> arguments;
        protected IActionExecutionEngine actionExecutionEngine;
        protected IPlanCatalog planCatalog;
        protected ActionPlan plan;        

        /// <summary>
        /// Method that takes the arguments and parses them to the Dictionary "arguments"
        /// </summary>
        /// <param name="args">The argument string to parse</param>
        /// <returns>0 if success else an error number</returns>
        public int ParseArguments(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            this.retVal = RETSUCCESS;
            Dictionary<string, object> d = new Dictionary<string, object>();
            foreach (string s in args)
            {
                if (s.Contains(":"))
                {
                    string command = s.Substring(1, s.IndexOf(':') - 1);
                    string value = s.Substring(s.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1);
                    if (value.Length == 0)
                    {
                        Console.WriteLine("Error: Argument cannot be empty: " + command);
                        this.retVal = ERROREMPTYARGUMENT;
                        break;
                    }

                    if (d.ContainsKey(command.ToUpper()))
                    {
                        Console.WriteLine("Error: Same argument cannot occur twice: " + command);
                        this.retVal = ERRORDOUBLEARGUMENT;
                        break;
                    }

                    var prop = this.GetProperty(command);
                    if (prop != null)
                    {
                        if (prop.TypeInformation == null)
                        {
                            d.Add(command.ToUpper(), value);
                        }
                        else if (prop.TypeInformation == "bool")
                        {
                            if (value.ToUpper() == "Y" || value.ToUpper() == "YES" || value.ToUpper() == "TRUE")
                            {
                                d.Add(command.ToUpper(), true);
                            }
                            else if (value.ToUpper() == "N" || value.ToUpper() == "NO" || value.ToUpper() == "FALSE")
                            {
                                d.Add(command.ToUpper(), false);
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Error: Can't convert {0} to boolean value", value));
                                this.retVal = ERRORARGUMENTNOTBOOL;
                            }

                            break;
                        }
                        else
                        {
                            d.Add(command.ToUpper(), value);
                        }
                    }
                    else
                    {
                        d.Add(command.ToUpper(), value);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Argument should contain ':' to be valid " + s);
                    this.retVal = ERRORARGUMENTWITHOUTSEMICOLON;
                }
            }

            this.arguments = d;
            return this.retVal;
        }

        /// <summary>
        /// Method that checks if the arguments is what is expected - both optional and non-optional arguments
        /// </summary>
        /// <returns>0 if OK, else error number</returns>
        public int Validate()
        {
            this.retVal = RETSUCCESS;
            List<string> argLst = new List<string>();
            argLst.AddRange(this.arguments.Keys);

            foreach (ActionPropertyInfo expected in this.expectedArguments)
            {
                if (!this.arguments.Keys.Contains(expected.Name.ToUpper()))
                {
                    Console.WriteLine("Error: Expected argument {0}. Arguments should start with '/'", expected.Name);
                    this.retVal = ERRORMISSINGARGUMENT;
                }
                else
                {
                    argLst.Remove(expected.Name.ToUpper());
                }
            }

            if (this.optionalArguments != null)
            {
                foreach (ActionPropertyInfo optional in this.optionalArguments)
                {
                    if (this.arguments.Keys.Contains(optional.Name.ToUpper()))
                    {
                        argLst.Remove(optional.Name.ToUpper());
                    }
                }
            }

            if (argLst.Count > 0 && this.retVal == 0)
            {
                this.retVal = ERRORUNEXPECTEDARGUMENT;
                foreach (string s in argLst)
                {
                    Console.WriteLine("Error: Unexpected argument : " + s);
                }
            }

            return this.retVal;
        }

        public virtual int Run()
        {
            return this.retVal;
        }

        public string CommandSyntax()
        {
            string s = string.Empty;
            foreach (ActionPropertyInfo expected in this.expectedArguments)
            {
                s += string.Format(@"/{0}: ", expected.Name);
            }

            if (this.optionalArguments != null)
            {
                foreach (ActionPropertyInfo optional in this.optionalArguments)
                {
                    s += string.Format(@"[/{0}: ]", optional.Name);
                }
            }

            return s; 
        }

        public int ShowUsage()
        {
            this.retVal = CommandBase.RETSHOWEDHELP;            

            Console.WriteLine(@"------------------------------------------------------------------------");
            Console.WriteLine(@"TfsBranchTool Command line utility  - (c) Community TFS Branch Tool ");
            Console.WriteLine(@"------------------------------------------------------------------------");
            Console.WriteLine(string.Empty);
            Console.WriteLine(@"The following is a list of the commands that are available. Type TfsBranchTool [command] /? to view help for a specific command.");
            Console.WriteLine(string.Empty);
            foreach (var item in this.validOperations)
            {
                Console.WriteLine(@"       " + item.Name);
            }

            Console.WriteLine(string.Empty);

            return this.retVal;
        }

        public int ShowValidUsage(string operation)
        {          
            if (this.retVal == 0)
            {
                this.retVal = CommandBase.RETSHOWEDHELP;
                Console.WriteLine(@"------------------------------------------------------------------------");
                Console.WriteLine(@"TfsBranchTool Command line utility  - (c) Community TFS Branch Tool ");
                Console.WriteLine(@"------------------------------------------------------------------------");
                Console.WriteLine(string.Empty);
                Console.WriteLine(@"The following is a list of the parameters for the '{0}'' operation.", operation);
                Console.WriteLine(string.Empty);
                foreach (var item in this.expectedArguments)
                {
                    Console.WriteLine(@"       /" + item.Name);
                    Console.WriteLine(@"                - " + item.Description);
                }

                foreach (var item in this.optionalArguments)
                {
                    Console.WriteLine(@"       /{0} (optional)", item.Name);
                    Console.WriteLine(@"                - " + item.Description);
                }

                Console.WriteLine(string.Empty);
            }

            return this.retVal;
        }

        public int GetValidProperties(string validOperation)
        {
            int ret = 0;
            this.expectedArguments = new List<ActionPropertyInfo>();
            this.optionalArguments = new List<ActionPropertyInfo>();
            
            ActionPlan aplan = this.planCatalog.GetPlan(validOperation);
            if (aplan == null)
            {
                Console.WriteLine(@"      Could not find the operation {0} - probably due to incorrect casing", validOperation);
                ret = ERROROPERATIONNOTFOUND;
            }
            else
            {
                var properties = aplan.Properties;
                foreach (ActionPropertyInfo prop in properties)
                {
                    if (prop.Optional)
                    {
                        this.optionalArguments.Add(prop);
                    }
                    else
                    {
                        this.expectedArguments.Add(prop);
                    }
                }
            }

            return ret;
        }

        protected object GetArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            if (this.arguments.Keys.Contains(argument.ToUpper()))
            {
                return this.arguments[argument.ToUpper()];
            }

            return null;
        }

        protected List<ActionPlan> GetValidOperations()
        {
            List<ActionPlan> ops = new List<ActionPlan>();

            foreach (ActionPlan planInfo in this.planCatalog.Plans)
            {
                ops.Add(planInfo);
            }

            return ops;
        }

        private ActionPropertyInfo GetProperty(string command)
        {
            var prop = this.expectedArguments.Where(x => x.Name.ToUpper() == command.ToUpper());
            if (prop.Count() > 0)
            {
                return prop.First();
            }

            prop = this.optionalArguments.Where(x => x.Name.ToUpper() == command.ToUpper());
            if (prop.Count() > 0)
            {
                return prop.First();
            }

            return null;
        }
    }
    
    public class BranchPlanBaseCommand : CommandBase
    {
        public BranchPlanBaseCommand()
        {            
            this.actionExecutionEngine = ActionExecutionEngineFactory.CreateActionExecutionEngine();
            this.planCatalog = CatalogFactory.CreateFileSystemCatalog();

            this.validOperations = this.GetValidOperations();
        }

        /// <summary>
        /// Initializes a new instance of the BranchPlanBaseCommand class. For test purpose - to be able to inject a fake IActionExecutionEngine
        /// </summary>
        /// <param name="iacEngine">injected ActionExecutionEngine</param>
        public BranchPlanBaseCommand(IActionExecutionEngine iacEngine, IPlanCatalog fakeplan)
        {
            this.actionExecutionEngine = iacEngine;
            this.planCatalog = fakeplan;
            this.validOperations = this.GetValidOperations();
        }

        public override int Run()
        {
            Dictionary<string, object> argumentsToPass = new Dictionary<string, object>(); 
            Console.WriteLine("{0} with arguments:", this.plan.Name);
            foreach (ActionPropertyInfo s in this.expectedArguments)
            {
                Console.WriteLine("{0}: {1}", s.Name, this.GetArgument(s.Name));
                argumentsToPass.Add(s.Name, this.GetArgument(s.Name));
            }

            foreach (ActionPropertyInfo s in this.optionalArguments)
            {
                var argValue = this.GetArgument(s.Name);
                Console.WriteLine("{0}(optional): {1} - default value '{2}'", s.Name, argValue, s.Value);

                if (argValue != null)
                {
                    argumentsToPass.Add(s.Name, this.GetArgument(s.Name));
                }
            }
            
            try
            {              
                this.actionExecutionEngine.OnProgressChanged += this.ActionExecutionEngine_OnProgressChanged;

                var cts = new System.Threading.CancellationTokenSource();

                Task t = actionExecutionEngine.ExecuteAsync(this.plan, argumentsToPass, cts.Token);
                t.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    Console.WriteLine(inner.Message);
                    inner = inner.InnerException;
                }

                this.retVal = -1;
            }

            return this.retVal;
        }

        public void ActionExecutionEngine_OnProgressChanged(object sender, ProgressChangedArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public int ValidateOperation(string operation)
        {
            this.retVal = 0;
            bool operationFound = false;

            foreach (ActionPlan p in this.validOperations)
            {
                if (p.Name.ToUpper() == operation.ToUpper())
                {
                    operationFound = true;
                    this.plan = p;                    
                }
            }

            if (!operationFound)
            {
                Console.WriteLine("Error: Operation {0} not supported", operation);
                this.retVal = CommandBase.ERRORWRONGOPERATION;
            }
            else
            {
                this.retVal = this.GetValidProperties(this.plan.Name);
            }

            return this.retVal;
        }
    }
}