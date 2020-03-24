#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

// OptionValueQuery.cs - queries an existing TsGui option

using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class OptionValueQuery: BaseQuery, ILinkingEventHandler
    {
        private FormattedProperty _formatter;

        public OptionValueQuery(XElement inputxml, ILinkTarget owner): base(owner)
        {
            this._ignoreempty = false;
            this.SetDefaults();
            this.LoadXml(inputxml);
            Director.Instance.LinkingLibrary.AddHandler(this._formatter.Name,this);
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
                return Director.Instance.LinkingLibrary.GetSourceOption(id)?.CurrentValue;
            }
            else { return null; }
        }

        public void OnLinkedSourceValueChanged()
        {
            this.ProcessQuery();
            this._linktarget?.RefreshValue();
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

            this._processingwrangler.NewResult();
            
            x = InputXml.Element("ID");
            if (x != null)
            {
                this._formatter = new FormattedProperty(x);
                this._processingwrangler.AddFormattedProperty(this._formatter);
            }
        }
    }
}
