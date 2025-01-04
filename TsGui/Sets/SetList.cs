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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;
using WindowsHelpers;

namespace TsGui.Sets
{
    public class SetList
    {
        private static List<string> _defaultPaths = new List<string>
        {
            AppDomain.CurrentDomain.BaseDirectory + "Files\\",
            AppDomain.CurrentDomain.BaseDirectory,
            Directory.GetCurrentDirectory() + "\\"
        };
        private string _file;
        private string _prefix;
        private int _countLength;
        private Set _parent;

        public SetList(XElement inputXml, Set Parent)
        {
            this._parent = Parent;
            this._file = XmlHandler.GetStringFromXml(inputXml, "File", null);
            this._prefix = XmlHandler.GetStringFromXml(inputXml, "Prefix", null);
            this._countLength = XmlHandler.GetIntFromXml(inputXml, "CountLength", 2);
        }

        public async Task<List<Variable>> ProcessAsync()
        {
            string filecontents = string.Empty;
            if (this._file.StartsWith("http://") || this._file.StartsWith("https://") || this._file.StartsWith("ftp://"))
            {
                filecontents = await Web.ReadStringAsync(this._file);
            }
            else
            {
                //Use full path if used
                if (this._file.Contains("\\\\") || this._file.Contains(":\\"))
                {
                    if (File.Exists(this._file))
                    {
                        filecontents = await IOHelpers.ReadFileAsync(this._file);
                    }
                }
                //otherwise use the lookup options
                else
                {
                    foreach (var dir in _defaultPaths)
                    {
                        string testpath = dir + this._file;
                        if (File.Exists(testpath))
                        {
                            filecontents = await IOHelpers.ReadFileAsync(testpath);
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(filecontents))
            {
                return new List<Variable>();
            }
            if (string.IsNullOrWhiteSpace(this._prefix))
            { 
                return this.ProcessStatic(filecontents); 
            }

            return this.ProcessDynamic(filecontents);
        }

        private List<Variable> ProcessStatic(string filecontents)
        {
            var variables = new List<Variable>();
            var lines = filecontents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    char[] separator = { '=' };
                    var parts = line.Split(separator, 2);
                    string name = parts.Length > 0 ? parts[0] : null;
                    string value = parts.Length > 1 ? parts[1] : null;
                    if (name != null)
                    {
                        variables.Add(new Variable(name, value, this._parent.Path));
                    }
                }
            }
            return variables;
        }

        private List<Variable> ProcessDynamic(string filecontents)
        {
            var variables = new List<Variable>();
            var lines = filecontents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int count = 0;
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    count++;
                    variables.Add(new Variable(this._prefix + count.ToString("D" + this._countLength), line, this._parent.Path));
                }
            }

            return variables;
        }
    }
}
