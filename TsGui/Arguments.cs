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

// Arguments.cs - objec to process and store command line arguments

using System;

namespace TsGui
{
    public class Arguments
    {
        public string ConfigFile { get; set; }

        public Arguments(string[] Args)
        {
            this.SetDefaults();
            this.ProcessArguments(Args);
        }

        private void ProcessArguments(string[] Args)
        {
            //string[] args = Environment.GetCommandLineArgs();
            if (Args.Length > 1)
            {
                for (int index = 1; index < Args.Length; index += 2)
                {
                    if (Args.Length < index+2) { throw new InvalidOperationException("Missing command line paramter after \"" + Args[index] + "\""); }
                    switch (Args[index].ToUpper())
                    {
                        case "-CONFIG":
                            this.ConfigFile = CompleteFilePath(Args[index + 1]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void SetDefaults()
        {
            string exefolder = AppDomain.CurrentDomain.BaseDirectory;
            this.ConfigFile = exefolder + @"Config.xml";
        }

        private string CompleteFilePath(string Input)
        {
            if (!Input.Contains("\\"))
            {
                string exefolder = AppDomain.CurrentDomain.BaseDirectory;
                return exefolder + @Input;
            }
            else { return Input; }
        }
    }
}
