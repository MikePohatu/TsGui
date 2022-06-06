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
using System.Linq;
using System.Text;
using Core.Logging;
using Core.Diagnostics;
using TsGui.Connectors.System;
using WindowsHelpers;

namespace TsGui.Connectors
{
    /// <summary>
    /// Outputs IOptions to the registry
    /// </summary>
    public class RegistryConnector: IVariableOutput
    {
        private List<Variable> variables = new List<Variable>();

        public void AddVariable(Variable Variable)
        {
            Log.Info($"Variable added: {Variable.Name}, Value: {Variable.Value}, Path: {Variable.Path}");
            this.variables.Add(Variable);
        }

        public void Release()
        {
            foreach (Variable variable in this.variables)
            {
                string path = variable.Path;

                if (string.IsNullOrWhiteSpace(path)) 
                {
                    if (string.IsNullOrWhiteSpace(path)) { Log.Error($"Path cannot be empty. Variable name {variable.Name}"); }
                    continue;
                }                    
                
                if (string.IsNullOrWhiteSpace(variable.Name)) { 
                    Log.Error($"Name cannot be empty. Variable path {variable.Path}");
                    continue;
                }

                try
                {
                        
                    if (path.StartsWith("HKCU")) { path = path.Replace("HKCU", "HKEY_CURRENT_USER"); }
                    else if (path.StartsWith("HKLM")) { path = path.Replace("HKCU", "HKEY_LOCAL_MACHINE"); }
                    else if (path.StartsWith("HKU")) { path = path.Replace("HKCU", "HKEY_USERS"); }

                    RegistryHelpers.SetStringValue(path, variable.Name, variable.Value);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Error setting registry item: {variable.Path}\\{variable.Name}");
                    throw new KnownException($"Error setting registry item: {variable.Path}\\{variable.Name}", e.Message);
                }
            }            
        }

        public void Init()
        { }
    }
}
