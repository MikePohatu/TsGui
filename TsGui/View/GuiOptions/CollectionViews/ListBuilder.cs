//    Copyright (C) 2016 Mike Pohatu

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
using TsGui.Queries;
using TsGui.Diagnostics.Logging;

namespace TsGui.View.GuiOptions.CollectionViews
{
    public class ListBuilder
    {
        private int _lastindex = 0;
        private CollectionViewGuiOptionBase _parent;
        private IDirector _director;
        private Dictionary<int, QueryPriorityList> _querylists = new Dictionary<int, QueryPriorityList>();
        private Dictionary<int, ListItem> _staticitems = new Dictionary<int, ListItem>();
        public List<ListItem> Items { get; set; }

        public ListBuilder(CollectionViewGuiOptionBase parent, IDirector director)
        {
            this._director = director;
            this._parent = parent;
        }

        public List<ListItem> Rebuild()
        {
            LoggerFacade.Debug("ListBuilder rebuild initialised");
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
                    qlist.ProcessAllQueries();
                    ResultWrangler wrangler = qlist.GetResultWrangler();
                    if (wrangler != null)
                    {
                        List<KeyValuePair<string, string>> kvlist = wrangler.GetKeyValueList();
                        foreach (KeyValuePair<string, string> kv in kvlist)
                        {
                            ListItem newoption = new ListItem(kv.Key, kv.Value, this._parent.ControlFormatting, this._parent, this._director);
                            if (string.IsNullOrWhiteSpace(newoption.Text) == false) { newlist.Add(newoption); } //ignore items with empty text 
                        }
                    }
                    i++;
                    continue;
                }
                i++;
            }
            this.Items = newlist;
            LoggerFacade.Debug("ListBuilder rebuild finished");
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
    }
}
