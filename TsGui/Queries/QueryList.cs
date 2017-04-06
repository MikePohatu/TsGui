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

// QueryList.cs - A list of queries 

using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.Queries
{
    public class QueryList
    {
        private List<IQuery> _queries = new List<IQuery>();
        private MainController _controller;

        public QueryList(MainController controller)
        {
            this._controller = controller;
        }

        public void LoadXml(XElement InputXml)
        {
            foreach (XElement x in InputXml.Elements("Query"))
            {
                this._queries.Add(QueryFactory.GetQueryObject(x, this._controller));
            }
        }

        public ResultWrangler GetResultWrangler()
        {
            foreach (IQuery query in this._queries)
            {
                ResultWrangler wrangler = query?.GetResultWrangler();
                if (wrangler != null) { return wrangler; }
            }

            return null;
        }
    }
}
