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

// Result.cs - encapsulates a result from a query i.e the properties, the key value, 
// and any branch/sub results associated with it

using System.Collections.Generic;

namespace TsGui.Queries
{
    public class Result
    {
        public List<FormattedProperty> Properties { get; set; } = new List<FormattedProperty>();
        public List<Result> SubResults { get; set; } = new List<Result>(); //used for tree structures
        public FormattedProperty KeyProperty { get; set; }

        public void Add(FormattedProperty newproperty)
        {
            if (this.KeyProperty == null) { this.KeyProperty = newproperty; }
            else { this.Properties.Add(newproperty); }
        }

        public List<FormattedProperty> GetAllFormattedProperties()
        {
            List<FormattedProperty> l = new List<FormattedProperty>();
            if (this.KeyProperty != null) { l.Add(this.KeyProperty); }
            l.AddRange(this.Properties);
            foreach (Result r in this.SubResults)
            { l.AddRange(r.GetAllFormattedProperties()); }
            return l;
        }
    }
}
