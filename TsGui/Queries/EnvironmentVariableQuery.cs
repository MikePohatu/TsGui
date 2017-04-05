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
    public class EnvironmentVariableQuery
    {
        private SccmConnector _sccmconnector;

        public EnvironmentVariableQuery(SccmConnector sccmconnector)
        {
            this._sccmconnector = sccmconnector;
        }

        //get and environmental variable, trying the sccm ts variables first
        public string GetEnvVar(string variablename)
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
        public ResultWrangler ProcessEnvironmentVariableQuery(XElement InputXml)
        {
            ResultWrangler wrangler = new ResultWrangler();
            ResultFormatter rf;
            XElement x;
            XAttribute xattrib;

            wrangler.NewSubList();

            x = InputXml.Element("Variable");
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

                rf.Input = this.GetEnvVar(rf.Name.Trim());
                wrangler.AddResultFormatter(rf);
            }

            return wrangler;
        }
    }
}
