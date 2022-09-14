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

// CompareQuery.cs - compare multiple queries and return a result


using MessageCrap;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class CompareQuery: BaseQuery, ILinkTarget
    {
        private QueryPriorityList _querylist;
        private string _truevalue = "TRUE";
        private string _falsevalue = "FALSE";

        public CompareQuery(XElement inputxml, ILinkTarget owner) : base(owner)
        {
            this._querylist = new QueryPriorityList(owner);
            this._processingwrangler.Separator = string.Empty;
            this._reprocess = true;
            this.LoadXml(inputxml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            
            foreach (XElement x in InputXml.Elements())
            {
                if (x.Name == "TrueValue") { this._truevalue = x.Value; }
                else if (x.Name == "FalseValue") { this._falsevalue = x.Value; }
                else
                {
                    IQuery newquery = QueryFactory.GetQueryObject(x, this._linktarget);
                    if (newquery != null) { this._querylist.AddQuery(newquery); }
                }
            }
        }

        public override Task<ResultWrangler> GetResultWrangler(Message message)
        {
            return this.ProcessQueryAsync(message);
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            //if someone hasn't supplied to queries to compare, just return null i.e. invalid result
            if (this._querylist.Queries.Count <2) { return null; } 

            ResultWrangler wrangler = new ResultWrangler();

            string first = (await this._querylist.Queries[0]?.GetResultWrangler(message))?.GetString();

            for (int i = 1; i < this._querylist.Queries.Count; i++)
            {
                string second = this._querylist.Queries[i].GetResultWrangler(message)?.Result.GetString();
                wrangler.NewResult();
                FormattedProperty prop = new FormattedProperty();
                prop.Name = "Result";

                if (first == second)
                {
                    prop.Input = this._truevalue;
                }
                else
                {
                    prop.Input = this._falsevalue;
                }
                wrangler.AddFormattedProperty(prop);
            }

            return wrangler;
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        { await this._linktarget.OnSourceValueUpdatedAsync(message); }
    }
}
