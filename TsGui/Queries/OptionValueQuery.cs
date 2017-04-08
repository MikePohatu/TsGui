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
using TsGui.Linking;

namespace TsGui.Queries
{
    public class OptionValueQuery: BaseQuery, ILinkingEventHandler
    {
        private MainController _controller;
        private ResultFormatter _formatter;
        private ILinkTarget _linktargetoption;

        public OptionValueQuery(XElement inputxml, MainController controller, ILinkTarget owner)
        {
            this._linktargetoption = owner;
            this._controller = controller;
            this.SetDefaults();
            this.LoadXml(inputxml);
            this._controller.LinkingLibrary.AddHandler(this._formatter.Name,this);
            this.ProcessQuery();
        }

        

        public override ResultWrangler ProcessQuery()
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

        public void OnLinkedSourceValueChanged(ILinkSource source, LinkingEventArgs e)
        {
            this.ProcessQuery();
            this._linktargetoption?.RefreshValue();
        }

        private void SetDefaults()
        {
            this._reprocess = true;
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
