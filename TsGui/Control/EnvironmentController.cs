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
using System;

using TsGui.Queries;
using TsGui.Validation;
using TsGui.Connectors;
using TsGui.Linking;

namespace TsGui
{
    public class EnvironmentController
    {
        private ITsVariableOutput _outputconnector;
        private SccmConnector _sccmconnector;
        private MainController _controller;

        public SccmConnector SccmConnector { get { return this._sccmconnector; } }
        public ITsVariableOutput OutputConnector { get { return this._outputconnector; } }

        public EnvironmentController(MainController maincontroller)
        { this._controller = maincontroller; }

        public bool Init()
        {
            return this.CreateSccmObject();
        }

        private bool CreateSccmObject()
        {
            try
            {
                this._sccmconnector = new SccmConnector();
                this._outputconnector = this._sccmconnector;
                return true;
            }
            catch
            {
                this._outputconnector = new TestingConnector();           
                return false;
            }
        }

        public void HideProgressUI()
        {
            this._sccmconnector.Hide(); //hide the tsprogessui window
        }

        public void AddVariable(TsVariable Variable)
        {
            this._outputconnector.AddVariable(Variable);
        }


        //public string GetStringValueFromList(XElement InputXml, ILinkingTarget targetoption)
        //{
        //    this._controller.LinkingLibrary.AddTarget(targetoption);

        //}

        /// <summary>
        /// Input a list of options as xml. Return the value of the first one that exists. 
        /// Return null if nothing is found. 
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        //public string GetStringValueFromList_old(XElement InputXml)
        //{
        //    string s = null;

        //    foreach (XElement x in InputXml.Elements())
        //    {
        //        IQuery newquery = QueryFactory.GetQueryObject(x, this._controller);
        //        if (newquery.Ignore == false)
        //        {
        //            s = newquery.GetResultWrangler().GetString();
        //            break;
        //        }
        //    }

        //    return s;
        //}


        /// <summary>
        /// Input a list of options as xml. Return a dictionary of results 
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        //public Dictionary<string, string> GetDictionaryFromList_old(XElement InputXml)
        //{
        //    return this.GetResultWranglerFromQuery(InputXml).GetDictionary();
        //}

        /// <summary>
        /// Input a list of options as xml. Return a List<KeyValuePair<string,<string>> of results 
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        //public List<KeyValuePair<string,string>> GetKeyValueListFromList_old(XElement InputXml)
        //{
        //    return this.GetResultWranglerFromQuery(InputXml).GetKeyValueList();
        //}


        //worker method for the public getdictionary and getkeyvaluelist methods above. builds and returns the wrangler that
        //can return the data in the right format. 
        //private ResultWrangler GetResultWranglerFromQuery_old(XElement InputXml)
        //{
        //    IQuery newquery = QueryFactory.GetQueryObject(InputXml, this._controller);
        //    return newquery.ProcessQuery();
        //}

        /// <summary>
        /// Process a <Query Type="GuiOption"> block and return the ResultWrangler
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        private ResultWrangler GetResultWranglerFromGuiOption_old(XElement InputXml)
        {
            ResultWrangler wrangler = new ResultWrangler();
            ResultFormatter rf;
            XElement x;
            XAttribute xattrib;

            wrangler.NewSubList();

            x = InputXml.Element("ID");
            if (x != null)
            {
                //check for new xml syntax. If the name attribute doesn't exist, setup for the 
                //legacy layout.
                xattrib = x.Attribute("Name");
                if (xattrib == null)
                {
                    rf = new ResultFormatter();
                    rf.Name = x.Value;
                }
                else
                {
                    rf = new ResultFormatter(x);
                }

                wrangler.AddResultFormatter(rf);
            }

            return wrangler;
        }

        //release the output connectors.
        public void Release()
        {
            this._outputconnector.Release();
        }


        private List<KeyValuePair<string, XElement>> GetTemplatesFromXmlElements(IEnumerable<XElement> Elements)
        {
            List<KeyValuePair<string, XElement>> templates = new List<KeyValuePair<string, XElement>>();

            foreach (XElement propx in Elements)
            {
                string name = propx.Attribute("Name")?.Value;
                //make sure there is a name set
                if (string.IsNullOrEmpty(name)) { throw new InvalidOperationException("Missing Name attribute in XML: " + Environment.NewLine + propx); }

                //add it to the templates list
                templates.Add(new KeyValuePair<string, XElement>(name, propx));
            }
            return templates;
        }
    }
}
