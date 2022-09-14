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
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public class Printer
    {
        public string Name { get; set; }
        public string Driver { get; set; }
        public string Path { get; set; }
        public string Username { get; set; }
        public string Server { get; set; }

        public Printer(PSObject posh)
        {
            this.Name = PoshHandler.GetPropertyValue<string>(posh, "Name");
            this.Driver = PoshHandler.GetPropertyValue<string>(posh, "Driver");
            this.Path = PoshHandler.GetPropertyValue<string>(posh, "Path");
            this.Username = PoshHandler.GetPropertyValue<string>(posh, "Username");
            this.Server = PoshHandler.GetPropertyValue<string>(posh, "Server");
        }
    }
}
