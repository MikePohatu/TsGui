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
using System.Xml.Linq;
using TsGui.Diagnostics.Logging;

namespace TsGui.Actions
{
    public static class ActionFactory
    {
        public static IAction CreateAction(XElement inputxml, IDirector director)
        {
            XAttribute xtype = inputxml.Attribute("Type");
            if (xtype == null) { throw new ArgumentException("Missing Type attribute on Action" + Environment.NewLine); }

            LoggerFacade.Info("Creating Action, type: " + xtype.Value);

            #region
            if (xtype.Value == "Authentication")
            {
                var action = new AuthenticationAction(inputxml, director);
                return action;
            }

            else
            { return null; }
            #endregion
        }
    }
}
