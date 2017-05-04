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

namespace TsGui.View.GuiOptions
{
    public class TsDropDownListBuilder
    {
        private int _lastindex = 0;
        private TsDropDownList _parentdropdown;
        private IDirector _director;
        private Dictionary<int, QueryPriorityList> _querylists = new Dictionary<int, QueryPriorityList>();
        private Dictionary<int, TsDropDownListItem> _staticitems = new Dictionary<int, TsDropDownListItem>();
        public List<TsDropDownListItem> Items { get; set; }

        public TsDropDownListBuilder(TsDropDownList parent, IDirector director)
        {
            this._director = director;
            this._parentdropdown = parent;
        }

        public List<TsDropDownListItem> Rebuild()
        {
            LoggerFacade.Debug("TsDropDownListBuilder rebuild initialised");
            int i = 0;
            List<TsDropDownListItem> newlist = new List<TsDropDownListItem>();
            while (i <= this._lastindex)
            {
                TsDropDownListItem staticitem;
                if (this._staticitems.TryGetValue(i,out staticitem) == true)
                {
                    newlist.Add(staticitem);
                    i++;
                    continue;
                }

                QueryPriorityList qlist;
                if (this._querylists.TryGetValue(i, out qlist) == true)
                {
                    ResultWrangler wrangler = qlist.GetResultWrangler();
                    if (wrangler != null)
                    {
                        List<KeyValuePair<string, string>> kvlist = wrangler.GetKeyValueList();
                        foreach (KeyValuePair<string, string> kv in kvlist)
                        {
                            TsDropDownListItem newoption = new TsDropDownListItem(kv.Key, kv.Value, this._parentdropdown.ControlFormatting, this._parentdropdown, this._director);
                            if (string.IsNullOrWhiteSpace(newoption.Text) == false) { newlist.Add(newoption); } //ignore items with empty text 
                        }
                    }
                    i++;
                    continue;
                }
                i++;
            }
            this.Items = newlist;
            LoggerFacade.Debug("TsDropDownListBuilder rebuild finished");
            return newlist;
        }

        public void Add(TsDropDownListItem item)
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
