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

// SccmConnector.cs - class to connect to the SCCM task sequence agent

using System;
using System.Runtime.InteropServices;
using TsGui.Diagnostics.Logging;


namespace TsGui.Connectors
{
    public class SccmConnector: ITsVariableOutput
    {
        dynamic objTSProgUI;
        dynamic objTSEnv;
        //bool hidden;

        public SccmConnector()
        {
            //hidden = false;
            objTSEnv = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TSEnvironment"));
            objTSProgUI = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TsProgressUI"));
        }

        public void AddVariable(TsVariable Variable)
        {
            LoggerFacade.Info("TS variable applied: " + Variable.Name + ". Value: " + Variable.Value);
            objTSEnv.Value[Variable.Name] = Variable.Value;
        }

        public void Hide()
        {           
            objTSProgUI.CloseProgressDialog();
            //this.hidden = true;
        }

        public void Release()
        {
            // Release the comm objects.
            if (Marshal.IsComObject(this.objTSProgUI) == true)
            {
                Marshal.ReleaseComObject(this.objTSProgUI);
            }

            if (Marshal.IsComObject(this.objTSEnv) == true)
            {
                Marshal.ReleaseComObject(this.objTSEnv);
            }
        }

        public string GetVariable(string Variable)
        {
            return objTSEnv.Value[Variable];
        }
    }
}
