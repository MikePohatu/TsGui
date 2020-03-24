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
using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class QueryPriorityList
    {
        private ILinkTarget _linktarget;
        private List<IQuery> _queries = new List<IQuery>();

        public List<IQuery> Queries { get { return this._queries; } }

        public QueryPriorityList(ILinkTarget linktarget)
        {
            this._linktarget = linktarget;
        }

        public QueryPriorityList(XElement inputxml, ILinkTarget linktarget)
        {
            this._linktarget = linktarget;
            this.LoadXml(inputxml);
        }

        public void AddQuery(IQuery query)
        {
            if (query == null) { return; }
            this._queries.Add(query);
        }

        public void AddQueryFirst(IQuery query)
        {
            if (query == null) { return; }
            this._queries.Insert(0,query);
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml != null)
            {
                foreach (XElement x in InputXml.Elements())
                {
                    IQuery newquery = QueryFactory.GetQueryObject(x, this._linktarget);
                    if (newquery != null) { this._queries.Add(newquery); }
                }
            }
        }

        public ResultWrangler GetResultWrangler()
        {
            foreach (IQuery query in this._queries)
            {
                ResultWrangler wrangler = query.GetResultWrangler();
                if (wrangler != null) { return wrangler; }
            }

            return null;
        }

        public void Clear()
        {
            this._queries.Clear();
        }

        public void ProcessAllQueries()
        {
            foreach (IQuery query in this._queries)
            { query.ProcessQuery(); }
        }

        public List<FormattedProperty> GetAllPropertyFormatters()
        {
            List<FormattedProperty> formatterlist = new List<FormattedProperty>();

            foreach (IQuery query in this._queries)
            {
                ResultWrangler wrangler = query.GetResultWrangler();
                if (wrangler != null) { formatterlist.AddRange(wrangler.GetAllPropertyFormatters()); }
            }
            return formatterlist;
        }
    }
}
