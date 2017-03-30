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


        public Logger()
        {
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this.LogToStdOut = true;
            this.LoggingLevel = LoggingLevels.Warning;
        }


        /// <summary>
        /// Process log of specified logging level/severity
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        public void WriteLog(string Message, int Level)
        {
            string fullmessage = LoggingLevels.ToShortString(Level) + ":" + DateTime.Now.ToString() + ": " + Message;

            if (Level >= this._loglevel)
            {
                if (this.LogToStdOut == true) { Console.WriteLine(fullmessage); }
                if (this._logtofile == true) { this.LogToFile(fullmessage); }
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

        private void LogToFile(string message)
        {

        }

        private void WriteToConsole(string Message)
        {
            Console.WriteLine(Message);
        }

        
    }
}
