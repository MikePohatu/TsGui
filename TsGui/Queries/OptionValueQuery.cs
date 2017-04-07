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

using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Validation;

namespace TsGui.Queries
{
    public class OptionValueQuery: BaseQuery, IQuery
    {
        private MainController _controller;
        private bool _processed = false;
        private ResultFormatter _formatter;
        private ResultWrangler _processingwrangler = new ResultWrangler();
        private ResultWrangler _returnwrangler;

        public OptionValueQuery(XElement inputxml, MainController controller)
        {
            this._controller = controller;
            this.LoadXml(inputxml);
            this.ProcessQuery();
        }

        public ResultWrangler GetResultWrangler()
        {
            if (this._processed == true) { return this._returnwrangler; }
            else { return this.ProcessQuery(); }
        }

        public ResultWrangler ProcessQuery()
        {
            this._formatter.Input = this.GetSourceOptionValue(this._formatter.Name.Trim());
            this._processed = true;
            return this.SetReturnWrangler();
        }

        public string GetSourceOptionValue(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return this._controller.LinkingLibrary.GetSourceOption(id)?.CurrentValue;
            }
            else { return null; }
        }

        private ResultWrangler SetReturnWrangler()
        {
            if (this.ShouldIgnore(this._formatter.Input) == true) { this._returnwrangler = null; }
            else { this._returnwrangler = this._processingwrangler; }
            return this._returnwrangler;
        }

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;

            this._processingwrangler.NewSubList();
            
            x = InputXml.Element("ID");
            if (x != null)
            {
                this._formatter = new ResultFormatter(x);
                this._processingwrangler.AddResultFormatter(this._formatter);
            }
        }
    }
}
