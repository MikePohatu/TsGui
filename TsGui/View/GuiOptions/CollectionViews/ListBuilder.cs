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
using System.Xml.Linq;
using System.Collections.Generic;
using TsGui.Queries;
using Core.Logging;
using TsGui.Queries.Trees;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class ListBuilder
    {
        private int _lastindex = 0;
        private CollectionViewGuiOptionBase _parent;
        private Dictionary<int, QueryPriorityList> _querylists = new Dictionary<int, QueryPriorityList>();
        private Dictionary<int, ListItem> _staticitems = new Dictionary<int, ListItem>();

        public List<ListItem> Items { get; set; }

        public ListBuilder(CollectionViewGuiOptionBase parent)
        {
            this._parent = parent;
        }

        public async Task<List<ListItem>> RebuildAsync(Message message)
        {
            Log.Debug("ListBuilder rebuild initialised");
            int i = 0;
            List<ListItem> newlist = new List<ListItem>();
            while (i <= this._lastindex)
            {
                ListItem staticitem;
                if (this._staticitems.TryGetValue(i,out staticitem) == true)
                {
                    newlist.Add(staticitem);
                    i++;
                    continue;
                }

                QueryPriorityList qlist;
                if (this._querylists.TryGetValue(i, out qlist) == true)
                {
                    await qlist.ProcessAllQueries(message);
                    ResultWrangler wrangler = await qlist.GetResultWrangler(message);
                    if (wrangler != null)
                    {
                        List<KeyValueTreeNode> kvlist = wrangler.GetKeyValueTree();
                        foreach (KeyValueTreeNode node in kvlist)
                        {
                            newlist.Add(this.CreateItem(node));
                        }
                    }
                    i++;
                    continue;
                }
                i++;
            }

            if (this._parent.Sort) {
                newlist.Sort();
                foreach (ListItem item in newlist)
                {
                    item.Sort();
                }
            }
            this.Items = newlist;

            Log.Debug("ListBuilder rebuild finished");
            return newlist;
        }

        public void Add(ListItem item)
        {
            this._lastindex++;
            this._staticitems.Add(this._lastindex, item);
        }

        public void Add(QueryPriorityList list)
        {
            this._lastindex++;
            this._querylists.Add(this._lastindex, list);
        }

        public void LoadXml(XElement inputxml)
        {
            foreach (XElement x in inputxml.Elements())
            {
                //read in an option and add to a dictionary for later use
                if (x.Name == "Option")
                {
                    ListItem newoption = new ListItem(x, this._parent.ControlStyle, this._parent);
                    this.Add(newoption);
                }

                else if (x.Name == "Query" || x.Name == "Value" || x.Name == "ListValue")
                {
                    XElement wrapx = new XElement("wrapx");
                    wrapx.Add(x);
                    QueryPriorityList newlist = new QueryPriorityList(this._parent);
                    newlist.LoadXml(wrapx);

                    this.Add(newlist);
                }
            }
        }

        public ListItem CreateItem(KeyValueTreeNode node)
        {
            ListItem newitem = new ListItem(node.Value.Key, node.Value.Value, this._parent.ControlStyle, this._parent);

            foreach (KeyValueTreeNode subnode in node.Nodes)
            {
                newitem.ItemsList.Add(this.CreateItem(subnode));
            }
            return newitem;
        }
    }
}
