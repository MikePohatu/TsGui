#region license
// Copyright (c) 2021 20Road Limited
//
// This file is part of DevChecker.
//
// DevChecker is free software: you can redistribute it and/or modify
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Logging;

namespace WindowsHelpers
{
    public static class AsyncHelpers
    {

        /// <summary>
        /// Create a process using the specified command and commandArgs, returning the resulting return code
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandArgs"></param>
        /// <returns></returns>
        public static Task<int> RunProcessAsync(string command, string commandArgs)
        {
            Process process = GetProcess(command, commandArgs);
            return StartProcessAsync(process);
        }

        public static Process GetProcess(string command, string commandArgs)
        {
            return GetProcess(command, commandArgs, true);
        }

        public static Process GetProcess(string command, string commandArgs, bool hideWindow)
        {
            Process process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = commandArgs;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = hideWindow;
            process.EnableRaisingEvents = true;

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                Log.Error(e.Data);
            };

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                Log.Info(e.Data);
            };

            return process;
        }

        /// <summary>
        /// Start a process created by GetProcess
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static Task<int> StartProcessAsync(Process process)
        {
            TaskCompletionSource<int> taskCompletion = new TaskCompletionSource<int>();

            process.Exited += (sender, args) =>
            {
                taskCompletion.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();

            return taskCompletion.Task;
        }
    }
}
