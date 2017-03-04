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
