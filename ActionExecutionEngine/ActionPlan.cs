// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ActionPlan.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// a ActionPlan object is the object that reads the Properties and descriptions for an action plan
    /// </summary>
    [XmlRoot("ActionPlan", IsNullable = false, Namespace = "http://schemas.microsoft.com/almrangers/2012/ActionPlan")]
    [Serializable]
    public class ActionPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPlan"/> class. 
        /// </summary>
        public ActionPlan()
        {
            this.Properties = new ActionPropertyInfo[0];
            this.Description = string.Empty;
            this.Actions = new ActionInfo[0];
            this.Name = string.Empty;
            this.Uri = string.Empty;
        }

        /// <summary>
        /// Gets/sets global properties for the plan.
        /// </summary>
        [XmlArray("properties", IsNullable = false)]
        [XmlArrayItem("property")]
        public ActionPropertyInfo[] Properties { get; set; }

        /// <summary>
        /// Gets/sets plan's description.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets actions list.
        /// </summary>
        [XmlArray("actions")]
        [XmlArrayItem("action")]
        public ActionInfo[] Actions { get; set; }

        /// <summary>
        /// Gets/sets name of the plan
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets URI of the HTML help file that contains extended user-friendly description of the plan.
        /// </summary>
        [XmlIgnore]
        public Uri HelpUri { get; set; }

        /// <summary>
        /// Helper property used for serialization of the <see cref="HelpUri"/>
        /// </summary>
        [XmlAttribute("helpUri")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string HelpUriString
        {
            get
            {
                return this.HelpUri == null ? null : this.HelpUri.ToString();
            }
            
            set
            {
                this.HelpUri = value == null ? null : new Uri(value, UriKind.RelativeOrAbsolute);
            }
        }

        /// <summary>
        /// Gets/sets <see cref="Uri"/> of the plan definition file.
        /// </summary>
        [XmlIgnore]
        public string Uri { get; set; }

        /// <summary>
        /// Loads plan definition from file and optionally validates it against schema.
        /// </summary>
        /// <param name="fileName">Name of the file that represents action plan.</param>
        /// <param name="schemaFileName">Name of the file that contains schema.</param>
        /// <returns><see cref="ActionPlan"/> object.</returns>
        public static ActionPlan Load(string fileName, string schemaFileName = null)
        {
            var serializer = new XmlSerializer(typeof(ActionPlan), "http://schemas.microsoft.com/almrangers/2012/ActionPlan");

            var readerSettings = new XmlReaderSettings();

            if (string.IsNullOrEmpty(schemaFileName))
            {
                readerSettings.ValidationType = ValidationType.None;
            }
            else
            {
                var schemas = new XmlSchemaSet();

                schemas.Add(null, schemaFileName);

                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.Schemas = schemas;
            }

            var reader = XmlReader.Create(fileName, readerSettings);

            using (reader)
            {
                var actionPlan = (ActionPlan)serializer.Deserialize(reader);

                actionPlan.Uri = fileName;

                if (actionPlan.HelpUri != null && !actionPlan.HelpUri.IsAbsoluteUri)
                {
                    var directoryName = Path.GetDirectoryName(actionPlan.Uri);
                    
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        actionPlan.HelpUri = new Uri(new Uri(directoryName + "/"), actionPlan.HelpUri);
                    }
                }

                actionPlan.Validate();

                return actionPlan;
            }
        }

        /// <summary>
        /// Checks plan's data for consistency
        /// </summary>
        /// <exception cref="InvalidDataException">There is no at least one action defined or optional global property contains no default value.</exception>
        public void Validate()
        {
            if (this.Actions == null || this.Actions.Length == 0)
            {
                throw new InvalidDataException("At least one action should be specified.");
            }

            foreach (var property in this.Properties.Where(property => property.Optional && string.IsNullOrEmpty(property.Value)))
            {
                throw new InvalidDataException(string.Format("Property '{0}' is not mandatory but it doesn't contain default value.", property.Name));
            }
        }
    }
}