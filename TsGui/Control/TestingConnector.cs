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

// TestingConnector.cs - class to use if SccmConnector can't connect to the 
// task sequence agent i.e. if not running from a task sequence (test mode)

using System;
using System.Collections.Generic;
using System.Windows;

using TsGui.Diagnostics.Logging;

namespace TsGui
{
    public class TestingConnector: ITsVariableOutput
    {
        private List<TsVariable> variables = new List<TsVariable>();

        public void AddVariable(TsVariable Variable)
        {
            LoggerFacade.Info("Testing variable applied: " + Variable.Name + ". Value: " + Variable.Value);
            this.variables.Add(Variable);
        }

        public void Release()
        {
            string msg = "Task sequence variables created:" + Environment.NewLine + Environment.NewLine;

            foreach (TsVariable variable in this.variables)
            {
                msg = msg + variable.Name + ": " + variable.Value + Environment.NewLine;
            }

            MessageBox.Show(msg);
        }

        public void Hide()
        { }
    }
}
