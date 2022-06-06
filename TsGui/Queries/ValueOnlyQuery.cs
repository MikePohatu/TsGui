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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Queries
{
    public class ValueOnlyQuery : IQuery
    {
        private FormattedProperty _formatter = new FormattedProperty();
        private ResultWrangler _wrangler = new ResultWrangler();

        public bool Ignore { get { return false; } }
        public string Value
        {
            get { return this._formatter.Input; }
            set { this._formatter.Input = value; }
        }
        public ValueOnlyQuery(XElement InputXml)
        {
            this._formatter.Input = InputXml.Value;
            this._wrangler.NewResult();
            this._wrangler.AddFormattedProperty(this._formatter);
        }

        public ValueOnlyQuery(string value)
        {
            this._formatter.Input = value;
            this._wrangler.NewResult();
            this._wrangler.AddFormattedProperty(this._formatter);
        }

        public async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            await Task.CompletedTask;
            return this._wrangler; 
        }

        public async Task<ResultWrangler> ProcessQuery(Message message)
        {
            await Task.CompletedTask;
            return this._wrangler; 
        }
    }
}
