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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Queries
{
    public class ListValueQuery: IQuery
    {
        private string _value = string.Empty;
        private string _text = string.Empty;

        public ListValueQuery(XElement InputXml)
        {
            this._value = XmlHandler.GetStringFromXml(InputXml, "Value", this._value);
            this._text = XmlHandler.GetStringFromXml(InputXml, "Text", this._value);

            this._formatter.Input = this._text;
            this._wrangler.NewResult();

            FormattedProperty valprop = new FormattedProperty();
            valprop.Input = this._value;

            this._wrangler.AddFormattedProperty(valprop);
            this._wrangler.AddFormattedProperty(this._formatter);
        }

        protected FormattedProperty _formatter = new FormattedProperty();
        protected ResultWrangler _wrangler = new ResultWrangler();

        public bool Ignore { get { return false; } }
        public string Value
        {
            get { return this._formatter.Input; }
            set { this._formatter.Input = value; }
        }

        public async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            await Task.CompletedTask;
            return this._wrangler; 
        }

        public async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            await Task.CompletedTask;
            return this._wrangler; 
        }
    }
}
