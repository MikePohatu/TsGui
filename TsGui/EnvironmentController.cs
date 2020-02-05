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
using TsGui.Diagnostics.Logging;

namespace TsGui
{
    public class EnvironmentController
    {
        private ITsVariableOutput _outputconnector;
        private SccmConnector _sccmconnector;
        private IDirector _controller;

        public SccmConnector SccmConnector { get { return this._sccmconnector; } }
        public ITsVariableOutput OutputConnector { get { return this._outputconnector; } }

        public EnvironmentController(IDirector maincontroller)
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
                LoggerFacade.Trace("Couldn't create SCCM connector. Creating testing connector");
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
