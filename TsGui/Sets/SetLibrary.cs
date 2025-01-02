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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TsGui.Sets
{
    public static class SetLibrary
    {
        public static ObservableCollection<Set> Sets { get; private set; } = new ObservableCollection<Set>();
        public static void Reset()
        {
            Sets.Clear();
        }

        public static void LoadXml(XElement inputXml)
        {
            if (inputXml == null) throw new ArgumentNullException("Sets xml is null");

            foreach (XElement x in inputXml.Elements("Set"))
            {
                Sets.Add(new Set(x));
            }
        }

        public async static Task<List<Variable>> ProcessAllAsync()
        {
            List<Variable> list = new List<Variable>();
            foreach (Set set in Sets)
            {
                if (set.IsActive == true && set.Enabled == true)
                {
                    var processed = await set.ProcessAsync();
                    list.AddRange(processed);
                }
            }
            return list;
        }
    }
}
