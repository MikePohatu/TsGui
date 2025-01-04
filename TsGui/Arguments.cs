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

// Arguments.cs - objec to process and store command line arguments

using System;

namespace TsGui
{
    public class Arguments
    {
        public bool Debug { get; private set; }

        public string ConfigFile { get; private set; }

        public string WebConfigUrl { get; private set; }
        public string LogFile { get; private set; }
        public int LoggingLevel { get; private set; }

        public bool TestMode { get; private set; }

        public bool CreateKey { get; private set; } = false;

        public string Key { get; private set; }

        public string ToHash { get; private set; }

        public void Import(string[] Args)
        {
            this.SetDefaults();
            this.ProcessArguments(Args);
        }

        public static Arguments Instance { get; } = new Arguments();

        private void ProcessArguments(string[] Args)
        {
            //string[] args = Environment.GetCommandLineArgs();
            if (Args.Length > 1)
            {
                for (int index = 1; index < Args.Length; index += 2)
                {                  
                    switch (Args[index].ToUpper())
                    {
                        case "-DEBUG":
                            this.Debug = true;
                            break;
                        case "-CONFIG":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing config file after parameter -config"); }
                            this.ConfigFile = this.CompleteFilePath(Args[index + 1]);                           
                            break;
                        case "-WEBCONFIG":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing URL after parameter -webconfig"); }
                            this.WebConfigUrl = Args[index + 1];
                            break;
                        case "-LOG":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing config file after parameter -log"); }
                            this.LogFile = this.CompleteFilePath(Args[index + 1]);
                            break;
                        case "-CREATEKEY":
                            this.CreateKey = true;
                            break;
                        case "-HASH":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing value after -hash"); }
                            this.ToHash = Args[index + 1];
                            break;
                        case "-KEY":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing value after -key"); }
                            this.Key = Args[index + 1];
                            break;
                        case "-TEST":
                            this.TestMode = true;
                            break;
                        default:
                            throw new InvalidOperationException("Invalid parameter: " + Args[index]);
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
