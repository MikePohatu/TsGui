using System.Xml.Linq;
using System.Diagnostics;
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

            Debug.WriteLine(InputXml);

            foreach (XElement x in InputXml.Elements())
            {
                if (string.Equals(x.Name.ToString(), "Query", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine("Query requested");
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
