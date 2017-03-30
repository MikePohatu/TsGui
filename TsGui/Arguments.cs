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

using TsGui.Diagnostics;

namespace TsGui
{
    public class Arguments
    {
        public string ConfigFile { get; set; }
        public string LogFile { get; set; }
        public int LoggingLevel { get; set; }

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
                    switch (Args[index].ToUpper())
                    {
                        case "-CONFIG":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing config file after parameter -config"); }
                            this.ConfigFile = this.CompleteFilePath(Args[index + 1]);                           
                            break;
                        case "-LOG":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing config file after parameter -log"); }
                            this.LogFile = this.CompleteFilePath(Args[index + 1]);
                            break;
                        case "-LOGGINGLEVEL":
                            if (Args.Length < index + 2) { throw new InvalidOperationException("Missing config file after parameter -log"); }
                            this.LoggingLevel = this.SetLoggingLevel(Args[index + 1]);
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

        private int SetLoggingLevel(string Input)
        {
            switch (Input.ToUpper())
            {
                case "DEBUG":
                    return LoggingLevels.Debug;
                case "INFORMATION":
                    return LoggingLevels.Information;
                case "INFO":
                    return LoggingLevels.Information;
                case "WARNING":
                    return LoggingLevels.Warning;
                case "WARN":
                    return LoggingLevels.Warning;
                case "ERROR":
                    return LoggingLevels.Error;
                case "ERR":
                    return LoggingLevels.Error;
                default:
                    throw new InvalidOperationException("Invalid logging level: " + Input);
            }
        }
    }
}
