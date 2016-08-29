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

using System.Xml.Linq;
using System.Collections.Generic;
//using System.Diagnostics;
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


        //input a list of options as xml. Return the value of the first one that exists. 
        //return null if nothing is found. 
        public Dictionary<string, string> GetDictionaryFromList(XElement InputXml)
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            string type;
            XElement x;

            type = InputXml.Attribute("Type")?.Value;
            
            if (string.IsNullOrEmpty(type)) { throw new InvalidOperationException("No type specified: " + Environment.NewLine + InputXml + Environment.NewLine); }

            if (string.Equals(type, "WmiQuery", StringComparison.OrdinalIgnoreCase))
            {

                string separator = ", ";
                string wql = InputXml.Element("Wql")?.Value;
                string keyprop = InputXml.Element("KeyProperty")?.Attribute("Name")?.Value;
                ResultFormatter keyProp;

                x = InputXml.Element("KeyProperty");
                if (x != null) { keyProp = new ResultFormatter(x); }

                x = InputXml.Element("Separator");
                if (x != null) { separator = x.Value; }

                if ((string.IsNullOrEmpty(wql)) || (string.IsNullOrEmpty(keyprop)))
                { throw new InvalidOperationException("Invalid config file. Missing Wql or KeyProperty from WMI query"); }
                else
                {
                    List<ResultFormatter> properties = new List<ResultFormatter>();
                    foreach (XElement prop in InputXml.Elements("Property"))
                    {
                        ResultFormatter propvalue = prop.Attribute("Name").Value;

                        properties.Add(propvalue);
                    }

                    dss = SystemConnector.GetWmiDictionary(wql, keyprop, separator, properties);

                }
            }
            return dss;
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
