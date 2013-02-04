// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionInfo.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   Represents action definition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.ALMRangers.BranchTool.ActionExecutionEngine
{
    using System;
    using System.Xml.Serialization;

    /// <summary>Represents action definition.</summary>
    [Serializable]
    public class ActionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInfo"/> class. 
        /// </summary>
        public ActionInfo()
        {
            this.Id = string.Empty;
            this.Name = string.Empty;
            this.InputProperties = new ActionPropertyInfo[0];
            this.OutputProperties = new ActionPropertyInfo[0];
        }

        #region Properties

        /// <summary>Gets/sets action ID.</summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>Gets/sets action name.</summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets <see cref="Array"/> of the <see cref="ActionPropertyInfo"/> that represents input properties.</summary>
        [XmlArray("input")]
        [XmlArrayItem("property")]
// ReSharper disable MemberCanBePrivate.Global
        public ActionPropertyInfo[] InputProperties { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        /// <summary>Gets or sets <see cref="Array"/> of the <see cref="ActionPropertyInfo"/> that represents  properties.</summary>
        [XmlArray("output")]
        [XmlArrayItem("property")]
// ReSharper disable MemberCanBePrivate.Global
        public ActionPropertyInfo[] OutputProperties { get; set; }
// ReSharper restore MemberCanBePrivate.Global
        #endregion
    }
}