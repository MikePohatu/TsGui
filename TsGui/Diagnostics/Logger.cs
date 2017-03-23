//    Copyright (C) 2017 Mike Pohatu

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

// Logger.cs - directs logging input to the correct output

using System;
using TsGui.Connectors.System;

namespace TsGui.Diagnostics
{
    public class Logger
    {
        private bool _logtofile;
        private string _logfile;
        private int _loglevel = LoggingLevels.Error;

        public int LoggingLevel
        {
            get { return this._loglevel; }
            set { this._loglevel = value; }
        }
        public bool LogToStdOut { get; set; }
        public string LogFile
        {
            get { return this._logfile; }
            set { this._logfile = value; this._logtofile = true; }
        }

        /// <summary>
        /// Process log of specified logging level/severity
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        public void WriteLog(string Message, int Level)
        {
            if (Level <= this._loglevel)
            {
                if (this.LogToStdOut == true) { Console.WriteLine(Message); }
                if (this._logtofile == true) { this.LogToFile(Message); }
            }        
        }

        /// <summary>
        /// Write a message to logging out. Will always output, no matter what LoggingLevel is set to
        /// </summary>
        /// <param name="Message"></param>
        public void WriteMessage(string Message)
        {
            if (this.LogToStdOut == true) { this.WriteToConsole(Message); }
            if (this._logtofile == true) { this.LogToFile(Message); }
        }

        private void LogToFile(string Message)
        {

        }

        private void WriteToConsole(string Message)
        {
            //Kernel32Methods.AttachConsole(-1);
            Console.WriteLine(Message);
            //Kernel32Methods.FreeConsole();
        }

        
    }
}
