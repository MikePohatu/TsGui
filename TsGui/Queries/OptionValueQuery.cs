//    Copyright (C) 2017 Mike Pohatu

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

// OptionValueQuery.cs - queries an existing TsGui option

using System.Xml.Linq;
using TsGui.Options;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class OptionValueQuery: BaseQuery, IQuery
    {
        private MainController _controller;
        private bool _processed = false;
        private ResultFormatter _formatter;
        private ResultWrangler _processingwrangler = new ResultWrangler();
        private ResultWrangler _returnwrangler;
        private IOption _owneroption;
        private ILinkingTarget _targetoption;

        public OptionValueQuery(XElement inputxml, MainController controller, IOption owner)
        {
            this._owneroption = owner;
            if (owner is ILinkingTarget)
            {
                this._targetoption = (ILinkingTarget)owner;
                
            }
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
            this._formatter.Input = this.GetSourceOptionValue(this._formatter.Name);
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

        public void OnLinkedSourceValueChanged(ILinkingSource source)
        {
            this.ProcessQuery();
            this._targetoption?.RefreshValue();
        }

        private ResultWrangler SetReturnWrangler()
        {
            if (this.ShouldIgnore(this._formatter.Value) == true) { this._returnwrangler = null; }
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
