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

// TsGuiKnownException - class for recording known exceptions to pass up the tree


using System;

namespace TsGui.Diagnostics
{
    public class TsGuiKnownException: Exception
    {
        public string CustomMessage { get; set; }

        public TsGuiKnownException(string CustomMessage, string SystemMessage):base(SystemMessage)
        {
            this.CustomMessage = CustomMessage;
        }
    }
}
