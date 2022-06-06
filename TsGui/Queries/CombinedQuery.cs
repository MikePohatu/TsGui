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

// CombinedQuery.cs - combine multiple queries


using MessageCrap;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class CombinedQuery: BaseQuery, ILinkTarget
    {
        private QueryPriorityList _querylist;

        public CombinedQuery(XElement inputxml, ILinkTarget owner) : base(owner)
        {
            this._querylist = new QueryPriorityList(owner);
            this._processingwrangler = new ResultWrangler();
            this._processingwrangler.Separator = string.Empty;
            this._reprocess = true;
            this.LoadXml(inputxml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            
            foreach (XElement x in InputXml.Elements())
            {
                IQuery newquery = QueryFactory.GetQueryObject(x, this._linktarget);
                if (newquery != null) { this._querylist.AddQuery(newquery); }
            }
        }

        public override Task<ResultWrangler> GetResultWrangler(Message message)
        {
            return this.ProcessQuery(message);
        }

        public override async Task<ResultWrangler> ProcessQuery(Message message)
        {
            this._processingwrangler = this._processingwrangler.Clone();
            this._processingwrangler.NewResult();
            this._processingwrangler.AddFormattedProperties(await this._querylist.GetAllPropertyFormatters(message));

            string s = this._processingwrangler.GetString();
            await Task.CompletedTask;
            if (this.ShouldIgnore(s) == false) { return this._processingwrangler; }
            else { return null; }
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        { await this._linktarget.OnSourceValueUpdatedAsync(message); }
    }
}
