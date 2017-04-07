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

using System.Xml.Linq;

namespace TsGui.Queries
{
    public class ValueOnly: IQuery
    {
        private ResultFormatter _formatter = new ResultFormatter();
        private ResultWrangler _wrangler = new ResultWrangler();

        public bool Ignore { get { return false; } }

        public ValueOnly(XElement InputXml)
        {
            this._formatter.Input = InputXml.Value;
            this._wrangler.AddResultFormatter(this._formatter);
        }

        public ResultWrangler GetResultWrangler()
        { return this._wrangler; }

        public ResultWrangler ProcessQuery()
        { return this._wrangler; }
    }
}
