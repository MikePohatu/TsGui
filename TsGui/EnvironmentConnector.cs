using System;
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
            if (this._sccmconnector != null) { this.AddVariable(Variable); }
            else { this._testconnector.AddVariable(Variable); }
        }

        //input a list of options as xml. Return the value of the first one that exists. 
        public string GetValueFromList(XElement InputXml)
        {
            #region
            string s;

            foreach (XElement x in InputXml.Elements())
            {
                if (x.Name == "EnvironmentVariable")
                {
                    s = this.GetEnvVar(x.Value);
                    if (s != null) { return s; }
                }  
                else if (x.Name == "WmiQuery")
                {
                    s = this.GetWmiQuery(x.Value);
                    if (s != null ) { return s; }
                }             
            }

            XText xt = InputXml.Nodes().OfType<XText>().FirstOrDefault();
            s = xt.Value.Trim();

            return s;

            #endregion
        }

        //get a value from WMI
        public string GetWmiQuery(string WmiQuery)
        {
            return null;
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
