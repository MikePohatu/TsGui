#region license
// Copyright (c) 2025 Mike Pohatu
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
using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using TsGui.Diagnostics;
using TsGui.Sets;

namespace TsGui.Options
{
    /// <summary>
    /// An OptionList consolidates a number of Options into a list, outputting to a list of
    /// variables using some prefix and an incrementing number. It is intended to be used with
    /// "Install applications according to dynamic variable list"
    /// https://learn.microsoft.com/en-us/mem/configmgr/osd/understand/task-sequence-steps#BKMK_InstallApplication
    /// </summary>
    public class OptionList
    {
        private string _prefix;
        private int _countLength = 2;
        private bool _useValue = false;
        private string _valueTest = "TRUE"; //value to compare to value of Option. If not the same ignore the option
        private List<IOption> _options = new List<IOption>();
        public string ID { get; private set; }

        public OptionList(string id)
        {
            this.ID = id;
            if (string.IsNullOrEmpty(this.ID)) { throw new KnownException("List missing ID attribute", ""); }

            this._prefix = this.ID;
        }

        public void LoadXml(XElement inputXml)
        {
            this._prefix = XmlHandler.GetStringFromXml(inputXml, "Prefix", this._prefix);
            if (string.IsNullOrEmpty(this._prefix)) { this._prefix = this.ID; }

            this._countLength = XmlHandler.GetIntFromXml(inputXml, "CountLength", this._countLength);
        }

        public void AddOption(IOption option)
        {
            this._options.Add(option);
        }

        public IEnumerable<Variable> GetVariables()
        {
            var variables = new List<Variable>();
            int count = 0;

            foreach (var option in this._options)
            {
                if (option.IsActive == true)
                {
                    count++;
                    if (this._useValue)
                    {
                        variables.Add(new Variable(this._prefix + count.ToString("D" + this._countLength), option.CurrentValue, option.Path));
                    }
                    else
                    {
                        //check if option value matches the test
                        if (option.CurrentValue.Equals(this._valueTest, StringComparison.OrdinalIgnoreCase))
                        {
                            variables.Add(new Variable(this._prefix + count.ToString("D" + this._countLength), option.VariableName, option.Path));
                        }
                    }
                }
            }
            return variables;
        }
    }
}
