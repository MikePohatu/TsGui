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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TsGui.Queries;
using TsGui.Diagnostics.Logging;

namespace TsGui.View.GuiOptions
{
    public class TsDropDownListBuilder
    {
        private int _lastindex = 0;
        private Dictionary<int, QueryList> _querylists = new Dictionary<int, QueryList>();
        private Dictionary<int, TsDropDownListItem> _staticitems = new Dictionary<int, TsDropDownListItem>();

        public List<TsDropDownListItem> Rebuild()
        {
            int i = 0;
            List<TsDropDownListItem> newlist = new List<TsDropDownListItem>();
            while (i <= this._lastindex)
            {

                i++;
            }

            return newlist;
        }
    }
}
