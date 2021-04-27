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

using MessageCrap;
using System;
using System.Xml.Linq;
using TsGui.Diagnostics;
using TsGui.Diagnostics.Logging;
using TsGui.Linking;
using TsGui.Options;

namespace TsGui.Queries
{
    public class OptionValueQuery: BaseQuery
    {
        private FormattedProperty _formatter;
        IOption _source;
        public OptionValueQuery(XElement inputxml, ILinkTarget owner): base(owner)
        {
            this._ignoreempty = false;
            this.SetDefaults();
            this.LoadXml(inputxml);
            Director.Instance.ConfigLoadFinished += this.InitLinking;
        }

        public void InitLinking(object sender, EventArgs e)
        {
            this._source = this.GetSourceOption(this._formatter.Name);
            LinkingHub.Instance.RegisterLinkTarget(this._linktarget, this._source);
        }

        public override ResultWrangler ProcessQuery(Message message)
        {
            this._formatter.Input = this._source?.CurrentValue;
            this._processed = true;

            return this.SetReturnWrangler();
        }

        public IOption GetSourceOption(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                IOption o = LinkingHub.Instance.GetSourceOption(id);
                if (o == null) { throw new TsGuiKnownException($"Unable to locate linked source ID: {id}\nAre you missing or mistyping an ID?", null); }
                
                return o;
            }
            else {
                throw new TsGuiKnownException($"Unable to locate linked source. ID cannot be empty", null);
            }
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
