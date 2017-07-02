//    Copyright (C) 2017 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// WmiQuery.cs - responsible for processing a wmi query

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Management;

using TsGui.Diagnostics;
using TsGui.Connectors;

namespace TsGui.Queries
{
    public class WmiQuery: BaseQuery
    {
        private List<KeyValuePair<string, XElement>> _propertyTemplates;
        private string _wql;

        public WmiQuery() { }

        public WmiQuery(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this._wql = InputXml.Element("Wql")?.Value;
            //make sure there is some WQL to query
            if (string.IsNullOrEmpty(this._wql)) { throw new InvalidOperationException("Empty WQL query in XML: " + Environment.NewLine + InputXml); }

            this._processingwrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);
 
            this._propertyTemplates = QueryHelpers.GetTemplatesFromXmlElements(InputXml.Elements("Property"));
        }

        public override ResultWrangler ProcessQuery()
        {
            //Now go through the management objects return from WMI, and add the relevant values to the wrangler. 
            //New sublists are created for each management object in the wrangler. 
            try
            {
                if (this._processed == true ) { this._processingwrangler = this._processingwrangler.Clone(); }
                this.AddWmiPropertiesToWrangler(this._processingwrangler, SystemConnector.GetWmiManagementObjectList(this._wql), this._propertyTemplates);
            }
            catch (ManagementException e)
            {
                throw new TsGuiKnownException("WMI query caused an error:" + Environment.NewLine + this._wql, e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            return this._returnwrangler;
        }

        private void AddWmiPropertiesToWrangler(ResultWrangler Wrangler, IEnumerable<ManagementObject> WmiObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            foreach (ManagementObject m in WmiObjectList)
            {
                Wrangler.NewResult();
                PropertyFormatter rf = null;

                //if properties have been specified in the xml, query them directly in order
                if (PropertyTemplates.Count != 0)
                {
                    foreach (KeyValuePair<string, XElement> template in PropertyTemplates)
                    {
                        rf = new PropertyFormatter(template.Value);
                        rf.Input = m.GetPropertyValue(template.Key)?.ToString();
                        Wrangler.AddPropertyFormatter(rf);
                    }
                }
                //if properties not set, add them all 
                else
                {
                    foreach (PropertyData property in m.Properties)
                    {
                        rf = new PropertyFormatter();
                        rf.Input = property.Value?.ToString();
                        Wrangler.AddPropertyFormatter(rf);
                    }
                }
            }
        }
    }
}
