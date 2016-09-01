//    Copyright (C) 2016 Mike Pohatu

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

// EnvironmentController.cs - responsible for getting the right information from the right connector
// in the right format so it can be passed back to the MainController. 

using System.Xml.Linq;
using System.Collections.Generic;
using System.Management;
using System;

namespace TsGui
{
    public class EnvironmentController
    {
        private SccmConnector _sccmconnector;
        private TestingConnector _testconnector;

        public bool Init()
        {
            return this.CreateSccmObject();
        }

        private bool CreateSccmObject()
        {
            try
            {
                this._sccmconnector = new SccmConnector();
                this._sccmconnector.Hide();
                return true;
            }
            catch
            {
                this._testconnector = new TestingConnector();           
                return false;
            }
        }


        public void AddVariable(TsVariable Variable)
        {
            if (this._sccmconnector != null) { this._sccmconnector.AddVariable(Variable); }
            else { this._testconnector.AddVariable(Variable); }
        }


        //input a list of options as xml. Return the value of the first one that exists. 
        //return null if nothing is found. 
        public string GetValueFromList(XElement InputXml)
        {
            string s = null;
            XAttribute xtype;

            //Debug.WriteLine(InputXml);

            foreach (XElement x in InputXml.Elements())
            {
                if (string.Equals(x.Name.ToString(), "Query", StringComparison.OrdinalIgnoreCase))
                {
                    //Debug.WriteLine("Query requested");
                    xtype = x.Attribute("Type");
                    if (xtype == null) { throw new NullReferenceException("Missing Type attribute XML: " + Environment.NewLine + x); }

                    if (string.Equals(xtype.Value, "EnvironmentVariable", StringComparison.OrdinalIgnoreCase))
                    {
                        string variable = x.Element("Variable").Value;
                        s = this.GetEnvVar(variable.Trim());
                        
                    }
                    else if (string.Equals(xtype.Value, "Wmi", StringComparison.OrdinalIgnoreCase))
                    {
                        string wql = x.Element("Wql").Value;
                        if (!string.IsNullOrEmpty(wql))
                        { s = SystemConnector.GetWmiString(wql); }
                        else { throw new InvalidOperationException("Invalid config file. Missing Wql from WMI query"); }                        
                    }

                    //now check any return value is valid before returning from method. 
                    if (!string.IsNullOrEmpty(s))
                    {
                        //if it shouldn't be ignored, return the value. Otherwise, carry on
                        if (Checker.ShouldIgnore(x, s) == false) { return s; }
                        else { s = null; }
                    }
                }  
                
                else if (string.Equals(x.Name.ToString(), "Value", StringComparison.OrdinalIgnoreCase))
                {
                    if (x.Value == null) { return string.Empty; }
                    else { return x.Value; }
                }
            }

            return s;
        }


        //input a list of options as xml. Return a dictionary of results 
        public Dictionary<string, string> GetDictionaryFromList(XElement InputXml)
        {
            Dictionary<string, string> returnDic = new Dictionary<string, string>();
            string type;
            XElement x;

            type = InputXml.Attribute("Type")?.Value;
            
            if (string.IsNullOrEmpty(type)) { throw new InvalidOperationException("No type specified: " + Environment.NewLine + InputXml + Environment.NewLine); }

            if (string.Equals(type, "WmiQuery", StringComparison.OrdinalIgnoreCase))
            {
                ResultWrangler wrangler = new ResultWrangler();
                Dictionary<string, XElement> propertyTemplates = new Dictionary<string, XElement>(); 
                string separator = ", ";             
                string wql = InputXml.Element("Wql")?.Value;

                x = InputXml.Element("Separator");
                if (x != null)
                { separator = x.Value; }
                


                //make sure there is some WQL to query
                if (string.IsNullOrEmpty(wql)) { throw new InvalidOperationException("Empty WQL query in XML: " + Environment.NewLine + InputXml); }

                ManagementObjectCollection wmiObjects = SystemConnector.GetWmiManagementObjects(wql);
                
                //first import the properties from the XML to the templates dictionary
                foreach (XElement propx in InputXml.Elements("Property"))
                {
                    string name = propx.Attribute("Name")?.Value;
                    //make sure there is a name set
                    if (string.IsNullOrEmpty(name)) { throw new InvalidOperationException("Missing name attribute in XML: " + Environment.NewLine + propx); }

                    //add it to the templates dictionary
                    propertyTemplates.Add(name, propx);
                }

                //Now go through the management objects return from WMI, and add the relevant values to the wrangler. 
                //New sublists are created for each management object in the wrangler. 
                foreach (ManagementObject m in SystemConnector.GetWmiManagementObjects(wql))
                {
                    wrangler.NewSubList();

                    foreach (string propname in propertyTemplates.Keys)
                    {
                        XElement template;
                        ResultFormatter rf;
                        string input = m.GetPropertyValue(propname).ToString();

                        if (propertyTemplates.TryGetValue(propname, out template))
                        {
                            rf = new ResultFormatter(template);
                            rf.Input = input;
                            wrangler.AddResultFormatter(rf);
                        }
                    }
                }
                returnDic = wrangler.GetDictionary(separator);
            }

            return returnDic;
        }


        //get and environmental variable, trying the sccm ts variables first
        public string GetEnvVar(string VariableName)
        {
            string s;

            if (!string.IsNullOrEmpty(VariableName))
            {
                //try ts env first
                if (this._sccmconnector != null)
                {
                    s = this._sccmconnector.GetVariable(VariableName);
                    if (!string.IsNullOrEmpty(s)) { return s; }
                }

                //if hasn't returned already, try system env variables
                s = SystemConnector.GetVariableValue(VariableName);
                if (!string.IsNullOrEmpty(s)) { return s; }
                else return null;
            }
            else { return null; }
        }


        //release the output connectors.
        public void Release()
        {
            if (this._sccmconnector != null) { this._sccmconnector.Release(); }
            if (this._testconnector != null) { this._testconnector.Release(); }
        }
    }
}
