// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ActionExecutionEngine.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Action execution engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Action execution engine.
    /// </summary>
    public class ActionExecutionEngine : Disposable, IActionExecutionEngine
    {
        #region Fields

        private readonly CompositionContainer addonsContainer;

        private IEnumerable<Lazy<IAction, IActionMetadata>> actions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionExecutionEngine"/> class.
        /// </summary>
        public ActionExecutionEngine()
        {
            var catalog = new AggregateCatalog();

            // ReSharper disable AssignNullToNotNullAttribute
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

            // ReSharper restore AssignNullToNotNullAttribute
            this.addonsContainer = new CompositionContainer(catalog);
            this.ReloadAvailableActions();
        }

        #endregion

        #region Public Events

        /// <summary>On progress changed event.</summary>
        public event EventHandler<ProgressChangedArgs> OnProgressChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>Asynchronously executes all actions in the <see cref="ActionPlan"/> that is specified in <paramref name="plan"/>.</summary>
        /// <param name="plan">The <see cref="ActionPlan"/> to execute.</param>
        /// <param name="properties">Collection of global properties. Names should match those returned by <see cref="ActionPlan.Properties"/>.</param>
        /// <param name="token">The cancellation token that can be used for cancelling execution.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task ExecuteAsync(ActionPlan plan, Dictionary<string, object> properties, CancellationToken token)
        {
            this.ThrowErrorIfDisposed();

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.Validate(plan);

            // Validate global properties values
            foreach (var property in properties.Where(property => (property.Value == null)))
            {
                throw new ArgumentOutOfRangeException("properties", string.Format("Property '{0}' must contain a value.", property.Key));
            }

            // inject values for non-mandatory properties if not specified.
            foreach (var property in plan.Properties.Where(property => property.Optional && !properties.ContainsKey(property.Name)))
            {
                properties.Add(property.Name, property.Value);
            }

            var propertyNames = plan.Properties.Select(propertyRow => propertyRow.Name).ToArray();

            if (propertyNames.Any(propertyName => !properties.ContainsKey(propertyName)))
            {
                const string MessageFormat = "The properties '{0}' are required. Properties that has been provided are '{1}'";
                string message = string.Format(MessageFormat, string.Join(", ", propertyNames), string.Join(", ", properties.Keys));
                
                throw new ArgumentOutOfRangeException("properties", message);
            }

            int progress = 0;

            var propertyCache = properties.ToDictionary(
                    globalProperty => GetPropertyMacroName(plan.GetType().Name, globalProperty.Key),
                    globalProperty => globalProperty.Value);

            IActionExecutionContext context = new ActionExecutionContext();

            foreach (var actionDefinition in plan.Actions)
            {
                this.ReportProgress(string.Format("Executing '{0}' operation ... ", actionDefinition.Id), progress);

                string actionName = actionDefinition.Name;
                IAction action = this.GetActionByName(actionName);

                // Set input properties
                SetActionInputPropertyValues(actionDefinition, action, propertyCache);

                // Execute action
                try
                {
                    await action.ExecuteAsync(context);
                }
                catch (Exception e)
                {
                    string message = string.Format("Execution of action '{0}' failed. Details: {1}", actionDefinition.Id, e.Message);
                    throw new InvalidOperationException(message, e);
                }

                // Collect output properties to be available for macro expansion
                GetActionOutputPropertyValues(actionDefinition, action, propertyCache);

                // Check if validation should be done 
                foreach (var property in actionDefinition.OutputProperties.Where(property => property.Validate))
                {
                    var propertyInfo = GetGetPropertyInfo(action.GetType(), property.Name, actionDefinition.Id);
                    var propertyValue = propertyInfo.GetValue(action);
                    var expectedValue = System.Convert.ChangeType(property.ExpectedValue, propertyInfo.PropertyType);

                    if (!propertyValue.Equals(expectedValue))
                    {
                        string message = property.ValidationMessage;
                        if (message == null)
                        {
                            string.Format("Action {0} fails validation, property {1} expected {2} but returned {3}", actionDefinition.Id, property.Name, property.ExpectedValue, propertyValue);
                        }
                        else
                        {
                            message = ExpandMacros(message, propertyCache).ToString();
                        }

                        throw new ArgumentOutOfRangeException("OutputProperties", message);
                    }
                }

                progress += 100 / plan.Actions.Length;
                token.ThrowIfCancellationRequested();
            }

            this.ReportProgress("Execution completed", 100);
        }

        /// <summary>Reloads all available action handlers.</summary>
        /// <exception cref="ObjectDisposedException">Object is disposed.</exception>
        public void ReloadAvailableActions()
        {
            this.ThrowErrorIfDisposed();

            lock (this.addonsContainer)
            {
                this.actions = this.addonsContainer.GetExports<IAction, IActionMetadata>();
            }
        }

        /// <summary>
        /// Validates <see cref="ActionPlan"/>
        /// </summary>
        /// <param name="plan"><see cref="ActionPlan"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Value of the <paramref name="plan"/> argument is null.</exception>
        public void Validate(ActionPlan plan)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }

            this.ThrowErrorIfDisposed();

            plan.Validate();

            foreach (var actionDefinition in plan.Actions)
            {
                string actionName = actionDefinition.Name;
                IAction action = this.GetActionByName(actionName);

                // Validate input properties
                foreach (var inPropertyRow in actionDefinition.InputProperties)
                {
                    GetSetPropertyInfo(action.GetType(), inPropertyRow.Name, actionDefinition.Id);
                }

                // Validate output properties
                foreach (var outPropertyRow in actionDefinition.OutputProperties)
                {
                    GetGetPropertyInfo(action.GetType(), outPropertyRow.Name, actionDefinition.Id);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary><para>Releases the unmanaged resources used by the parent class and optionally releases the managed resources.</para>
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.addonsContainer.Dispose();
            }

            base.Dispose(disposing);
        }

        private static void GetActionOutputPropertyValues(ActionInfo actionDefinition, IAction action, Dictionary<string, object> propertyCache)
        {
            foreach (var outPropertyRow in actionDefinition.OutputProperties)
            {
                var propertyName = outPropertyRow.Name;
                var propertyInfo = GetGetPropertyInfo(action.GetType(), propertyName, actionDefinition.Id);
                var propertyValue = propertyInfo.GetValue(action);
                propertyCache.Add(GetPropertyMacroName(actionDefinition.Id, propertyName), propertyValue);
            }
        }

        private static string GetPropertyMacroName(string actionId, string propertyName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", actionId, propertyName);
        }

        private static PropertyInfo GetGetPropertyInfo(IReflect actionType, string propertyName, string actionId)
        {
            var propertyInfo = actionType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.IgnoreCase);

            if (propertyInfo == null || (propertyInfo.GetMethod.Attributes & MethodAttributes.Public) != MethodAttributes.Public)
            {
                throw new InvalidDataException(
                    string.Format(
                        "Add-on '{0}' doesn't implement public getter for property '{1}' but output property is required for action '{2}'.", 
                        actionType, 
                        propertyName, 
                        actionId));
            }

            return propertyInfo;
        }

        private static PropertyInfo GetSetPropertyInfo(IReflect actionType, string propertyName, string actionId)
        {
            var propertyInfo = actionType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.IgnoreCase);

            if (propertyInfo == null || (propertyInfo.SetMethod.Attributes & MethodAttributes.Public) != MethodAttributes.Public)
            {
                throw new InvalidDataException(
                    string.Format(
                        "Add-on '{0}' doesn't implement public setter for property '{1}' but input property is required for action '{2}'.", 
                        actionType, 
                        propertyName, 
                        actionId));
            }

            return propertyInfo;
        }

        private static void SetActionInputPropertyValues(ActionInfo actionDefinition, IAction action, Dictionary<string, object> propertyCache)
        {
            foreach (var property in actionDefinition.InputProperties)
            {
                var propertyInfo = GetSetPropertyInfo(action.GetType(), property.Name, actionDefinition.Id);

                object expandedValue;
                
                try
                {
                    expandedValue = ExpandMacros(property.Value, propertyCache);
                }
                catch (Exception e)
                {
                    var message = string.Format("Can't expand macros for property '{0}' in action '{1}'. {2}", property.Name, actionDefinition.Id, e.Message);
                    throw new InvalidDataException(message, e);
                }

                if (!propertyInfo.PropertyType.IsInstanceOfType(expandedValue))
                {
                    expandedValue = Convert.ChangeType(expandedValue, propertyInfo.PropertyType);
                }

                propertyInfo.SetValue(action, expandedValue);
            }
        }

        private static object ExpandMacros(object value, Dictionary<string, object> propertyCache)
        {
            object expandedValue = value;
            
            foreach (Match match in Regex.Matches(value.ToString(), @"\$\((\w+\.\w+)\)"))
            {
                string macroName = match.Groups[1].Value;

                if (!propertyCache.ContainsKey(macroName))
                {
                    var message = string.Format("Unable to expand macro '{0}' - identifier '{1}' couldn't be resolved.", match.Value, macroName);
                    throw new InvalidDataException(message);
                }

                object cachedValue;

                try
                {
                    cachedValue = ExpandMacros(propertyCache[macroName], propertyCache);
                }
                catch (Exception e)
                {
                    var message = string.Format("Can't assign property '{0}' value. {1}", macroName, e.Message);
                    throw new InvalidDataException(message, e);
                }

                if (match.Value == value.ToString())
                {
                    // assuming full property value replacement
                    expandedValue = cachedValue;
                    break;
                }

                expandedValue = ((string)expandedValue).Replace(match.Value, cachedValue.ToString());
            }

            return expandedValue;
        }

        private IAction GetActionByName(string actionName)
        {
            this.ThrowErrorIfDisposed();

            Lazy<IAction, IActionMetadata> actionData =
                this.actions.Where(plugin => plugin.Metadata.Name == actionName).DefaultIfEmpty(null).FirstOrDefault();

            if (actionData == null)
            {
                throw new InvalidDataException(
                    string.Format("Add-on that implement action with name '{0}' couldn't be found.", actionName));
            }

            return actionData.Value;
        }

        private void ReportProgress(string progressMessage, int progress)
        {
            if (this.OnProgressChanged != null)
            {
                var changedArgs = new ProgressChangedArgs { Message = progressMessage, Progress = progress };

                this.OnProgressChanged(this, changedArgs);
            }
        }

        #endregion
    }
}