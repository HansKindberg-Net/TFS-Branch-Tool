// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionPropertyInfo.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Defines the ActionPropertyInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents information about plan's or action's property.
    /// </summary>
    [Serializable]
    public class ActionPropertyInfo
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPropertyInfo"/> class. 
        /// </summary>
        public ActionPropertyInfo()
        {
            this.Name = string.Empty;
            this.Optional = false;
            this.Description = string.Empty;
            this.TypeInformation = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets property description.</summary>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the property name.</summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets a value indicating whether property is optional.</summary>
        [XmlAttribute("optional")]
        public bool Optional { get; set; }

        /// <summary> Gets or sets property type information.</summary>
        [XmlAttribute("TypeInformation")]
        public string TypeInformation { get; set; }

        /// <summary> Gets or sets property Validation information, only enforced for output properties</summary>
        [XmlAttribute("Validate")]
        public bool Validate { get; set; }

        /// <summary> Gets or sets property ExpectedValue information, only enforced for output properties.</summary>
        [XmlAttribute("ExpectedValue")]
        public string ExpectedValue { get; set; }

        /// <summary> Gets or sets ValidationMessage, only enforced for output properties.</summary>
        [XmlAttribute("ValidationMessage")]
        public string ValidationMessage { get; set; }

        /// <summary>Gets or sets the default value for optional properties.</summary>
        [XmlText]
        public string Value { get; set; }

        #endregion
    }
}