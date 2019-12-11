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


using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class QueryPriorityList
    {
        private ILinkTarget _owneroption;
        private List<IQuery> _queries = new List<IQuery>();

        public List<IQuery> Queries { get { return this._queries; } }

        public QueryPriorityList(ILinkTarget owner)
        {
            this._owneroption = owner;
        }

        public QueryPriorityList(XElement inputxml, ILinkTarget owner)
        {
            this._owneroption = owner;
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
                    IQuery newquery = QueryFactory.GetQueryObject(x, Director.Instance, this._owneroption);
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
