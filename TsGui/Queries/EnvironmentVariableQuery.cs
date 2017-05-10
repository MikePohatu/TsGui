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

// EnvironmentVariableQuery.cs - queries environment variables through the desired logic (try sccm, then proces, etc etc)

using System.Xml.Linq;
using TsGui.Connectors;

namespace TsGui.Queries
{
    public class EnvironmentVariableQuery: BaseQuery, IQuery
    {
        private SccmConnector _sccmconnector;
        private PropertyFormatter _formatter;

        public EnvironmentVariableQuery(XElement inputxml, SccmConnector sccmconnector)
        {
            this._sccmconnector = sccmconnector;
            this.LoadXml(inputxml);
        }

        //get and environmental variable, trying the sccm ts variables first
        public string GetEnvironmentVariableValue(string variablename)
        {
            string s;

            if (!string.IsNullOrEmpty(variablename))
            {
                //try ts env first
                if (this._sccmconnector != null)
                {
                    s = this._sccmconnector.GetVariableValue(variablename);
                    if (!string.IsNullOrEmpty(s)) { return s; }
                }

                //if hasn't returned already, try system env variables
                s = SystemConnector.GetVariableValue(variablename);
                if (!string.IsNullOrEmpty(s)) { return s; }
                else return null;
            }
            else { return null; }
        }



        /// <summary>
        /// Process a <Query Type="EnvironmentVariable"> block and return the ResultWrangler
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        public override ResultWrangler ProcessQuery()
        {
            this._formatter.Input = this.GetEnvironmentVariableValue(this._formatter.Name.Trim());
            this._processed = true;
            return this.SetReturnWrangler();         
        }

        private ResultWrangler SetReturnWrangler()
        {
            if (this._formatter.Input == null) { return null; }
            if (this.ShouldIgnore(this._formatter.Input) == true) { this._returnwrangler = null; }
            else { this._returnwrangler = this._processingwrangler; }
            return this._returnwrangler;
        }

        

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;
            XAttribute xattrib;

            this._processingwrangler.NewResult();
            
            x = InputXml.Element("Variable");
            if (x != null)
            {
                //check for new xml syntax. If the name attribute doesn't exist, setup for the 
                //legacy layout.
                xattrib = x.Attribute("Name");
                if (xattrib == null)
                {
                    this._formatter = new PropertyFormatter();
                    this._formatter.Name = x.Value;
                }
                else
                {
                    this._formatter = new PropertyFormatter(x);
                }

                this._processingwrangler.AddResultFormatter(this._formatter);
            }
        }
    }
}
