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

namespace TsGui.Queries
{
    public class SourceOptionValueQuery: IQuery
    {
        private MainController _controller;
        private bool _processed = false;
        private ResultFormatter _formatter;
        private ResultWrangler _wrangler = new ResultWrangler();

        public SourceOptionValueQuery(XElement inputxml, MainController controller)
        {
            this._controller = controller;
            this.LoadXml(inputxml);
        }

        public ResultWrangler GetResultWrangler()
        {
            if (this._processed == true) { return this._wrangler; }
            else { return this.ProcessQuery(); }
        }

        public ResultWrangler ProcessQuery()
        {
            this._formatter.Input = this.GetSourceOptionValue(this._formatter.Name.Trim());
            this._processed = true;
            return this._wrangler;
        }

        public string GetSourceOptionValue(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return this._controller.LinkingLibrary.GetSourceOption(id)?.CurrentValue;
            }
            else { return null; }
        }

        private void LoadXml(XElement InputXml)
        {
            XElement x;

            this._wrangler.NewSubList();
            
            x = InputXml.Element("ID");
            if (x != null)
            {
                this._formatter = new ResultFormatter(x);
                this._wrangler.AddResultFormatter(this._formatter);
            }
        }
    }
}
