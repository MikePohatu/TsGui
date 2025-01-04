#region license
// Copyright (c) 2025 Mike Pohatu
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

namespace Core
{
    /// <summary>
    /// Create lists of items based on a maximum count, overflowing into new 'columns' of data when max
    /// is reached
    /// </summary>
    public class Overflow
    {
        public static List<IDictionary<T, N>> CreateFromDictionary<T,N>(IDictionary<T,N> dic, int maxRows)
        {            
            List<IDictionary<T, N>> columns = new List<IDictionary<T, N>>();
            Dictionary<T, N> column = new Dictionary<T, N>();
            int colcount = 0;
            columns.Add(column);

            foreach(var key in dic.Keys)
            {
                if (colcount == maxRows)
                {
                    column = new Dictionary<T, N>();
                    columns.Add(column);
                    colcount = 0;
                }
                column.Add(key, dic[key]);
                colcount++;
            }

            return columns;
        }

    }
}
