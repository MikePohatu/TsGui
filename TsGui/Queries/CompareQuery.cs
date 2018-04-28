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

// CompareQuery.cs - compare multiple queries and return a result


using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class CompareQuery: BaseQuery, ILinkTarget, ILinkingEventHandler
    {
        private QueryPriorityList _querylist;
        private IDirector _controller;
        private ILinkTarget _linktargetoption;
        private string _truevalue = "TRUE";
        private string _falsevalue = "FALSE";

        public CompareQuery(XElement inputxml, IDirector controller, ILinkTarget targetoption)
        {
            this._querylist = new QueryPriorityList(this, controller);
            this._processingwrangler.Separator = string.Empty;
            this._reprocess = true;
            this._controller = controller;
            this._linktargetoption = targetoption;
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
                    IQuery newquery = QueryFactory.GetQueryObject(x, this._controller, this);
                    if (newquery != null) { this._querylist.AddQuery(newquery); }
                }
            }
        }

        public override ResultWrangler GetResultWrangler()
        {
            return this.ProcessQuery();
        }

        public override ResultWrangler ProcessQuery()
        {
            ResultWrangler wrangler = new ResultWrangler();

            string first = this._querylist.Queries[0]?.GetResultWrangler()?.GetString();

            for (int i = 1; i < this._querylist.Queries.Count; i++)
            {
                string second = this._querylist.Queries[i].GetResultWrangler()?.GetString();
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

        public void RefreshValue()
        {
            this._linktargetoption.RefreshValue();
        }

        public void RefreshAll()
        { this._linktargetoption.RefreshAll(); }

        public void OnLinkedSourceValueChanged()
        { this.RefreshValue(); }
    }
}
