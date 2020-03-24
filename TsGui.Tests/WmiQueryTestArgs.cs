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
using System.Management;
using TsGui.Queries;

namespace TsGui.Tests
{
    public class WmiQueryTestArgs
    {
        //{ expectedresult, wrangler, objcollection, proptemplates };
        public string ExpectedResult { get; set; }
        public ResultWrangler Wrangler { get; set; }
        public List<ManagementObject> ManagementObjectList { get; set; }
        public List<KeyValuePair<string, XElement>> PropertyTemplates { get; set; }

        public WmiQueryTestArgs (string ExpectedResult, ResultWrangler Wrangler, List<ManagementObject> ManagementObjectList, List<KeyValuePair<string, XElement>> PropertyTemplates)
        {
            this.ExpectedResult = ExpectedResult;
            this.Wrangler = Wrangler;
            this.ManagementObjectList = ManagementObjectList;
            this.PropertyTemplates = PropertyTemplates;
        }
    }
}
