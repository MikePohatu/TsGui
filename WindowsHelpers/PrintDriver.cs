#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
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
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public class PrintDriver
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string PrinterEnvironment { get; set; }
        public string MajorVersion { get; set; }

        public PrintDriver(PSObject obj)
        {
            this.Name = PoshHandler.GetPropertyValue<string>(obj, "Name");
            this.Manufacturer = PoshHandler.GetPropertyValue<string>(obj, "Manufacturer");
            this.PrinterEnvironment = PoshHandler.GetPropertyValue<string>(obj, "PrinterEnvironment");
            this.MajorVersion = PoshHandler.GetPropertyValue<string>(obj, "MajorVersion");
        }
    }
}
