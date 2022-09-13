#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

// WmiQuery.cs - responsible for processing a wmi query

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Management;

using Core.Diagnostics;
using TsGui.Connectors;
using TsGui.Linking;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Queries
{
    public class WmiQuery: BaseQuery
    {
        private string _namespace = @"root\CIMV2";
        private List<KeyValuePair<string, XElement>> _propertyTemplates;
        private string _wql;

        public WmiQuery(ILinkTarget owner) : base(owner) { }

        public WmiQuery(XElement InputXml, ILinkTarget owner) : base(owner)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this._wql = InputXml.Element("Wql")?.Value;
            //make sure there is some WQL to query
            if (string.IsNullOrEmpty(this._wql)) { throw new InvalidOperationException("Empty WQL query in XML: " + Environment.NewLine + InputXml); }

            this._namespace = XmlHandler.GetStringFromXElement(InputXml, "NameSpace", this._namespace);
            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);
 
            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            //Now go through the management objects return from WMI, and add the relevant values to the wrangler. 
            //New sublists are created for each management object in the wrangler. 
            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }
                this.AddWmiPropertiesToWrangler(this._processingwrangler, SystemConnector.GetWmiManagementObjectList(this._namespace, this._wql), this._propertyTemplates);
            }
            catch (ManagementException e)
            {
                throw new KnownException("WMI query caused an error:" + Environment.NewLine + this._wql, e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            await Task.CompletedTask;

            return this._returnwrangler;
        }

        private void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            foreach (ManagementObject m in WmiObjectList)
            {
                Wrangler.NewResult();
                FormattedProperty prop = null;

                //if properties have been specified in the xml, query them directly in order
                if (PropertyTemplates.Count != 0)
                {
                    foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                    {
                        prop = new FormattedProperty(template.Value);
                        prop.Input = m.GetPropertyValue(template.Key)?.ToString();
                        Wrangler.AddFormattedProperty(prop);
                    }
                }
                //if properties not set, add them all 
                else
                {
                    foreach (PropertyData property in m.Properties)
                    {
                        prop = new FormattedProperty();
                        prop.Input = property.Value?.ToString();
                        Wrangler.AddFormattedProperty(prop);
                    }
                }
            }
        }
    }
}
