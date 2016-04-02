using System;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Windows;

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
        public string GetValueFromList(XElement InputXml)
        {
            string s = null;
            
            foreach (XElement x in InputXml.Elements())
            {
                if (x.Name == "EnvironmentVariable")
                {
                    s = this.GetEnvVar(x.Value.Trim());
                    if (s != null)
                    {
                        //if it shouldn't be ignored, return the value. Otherwise, carry on
                        if (Checker.ShouldIgnore(x, s) == false) { return s; }
                        else { s = null; }
                    }
                }  
                else if (x.Name == "WmiQuery")
                {
                    string query = x.Element("Query").Value;
                    s = SystemConnector.GetWmiString(query);
                    if (s != null )
                    {
                        //if it shouldn't be ignored, return the value. Otherwise, carry on
                        if (Checker.ShouldIgnore(x,s) == false) { return s; }
                        else { s = null;  }                    
                    }
                }             
            }

            s = string.Concat(InputXml.Nodes().OfType<XText>()).Trim();

            return s;
        }


        //get and environmental variable, trying the sccm ts variables first
        private string GetEnvVar(string VariableName)
        {
            string s;

            if (VariableName != null)
            {
                //try ts env first
                if (this._sccmconnector != null)
                {
                    s = this._sccmconnector.GetVariable(VariableName);
                    if (s != null) { return s; }
                }

                //if hasn't returned already, try system env variables
                s = SystemConnector.GetVariableValue(VariableName);
                return s;
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
