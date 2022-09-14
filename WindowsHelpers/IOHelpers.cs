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
using Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public static class IOHelpers
    {
        public async static Task<string> ReadFileAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found: " + path);
            }

            string script = string.Empty;
            try
            {
                byte[] result;
                Encoding encoding = Encoding.UTF8;
                using (var reader = new StreamReader(path, Encoding.UTF8, true))
                {
                    reader.Peek();
                    encoding = reader.CurrentEncoding;
                }

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    result = new byte[fs.Length];
                    await fs.ReadAsync(result, 0, (int)fs.Length);
                }
                //get the string and strip the BOM
                //script = encoding.GetString(result).Trim(new char[] { '\uFEFF', '\u200B' });
                script = GetString(encoding, result);
                return script;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to load file: " + path);
            }

            return script;
        }

        //https://stackoverflow.com/a/29176183
        public static string GetString(Encoding encoding, byte[] bytes)
        {
            byte[] preamble = encoding.GetPreamble();
            if (bytes.StartsWith(preamble))
            {
                return encoding.GetString(bytes, preamble.Length, bytes.Length - preamble.Length);
            }
            else
            {
                return encoding.GetString(bytes);
            }
        }

        //https://stackoverflow.com/a/29176183
        public static bool StartsWith(this byte[] thisArray, byte[] otherArray)
        {
            // Handle invalid/unexpected input
            // (nulls, thisArray.Length < otherArray.Length, etc.)

            for (int i = 0; i < otherArray.Length; ++i)
            {
                if (thisArray[i] != otherArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Write a text file with UTF-8 Encoding
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async static Task WriteTextFileAsync(string path, string text)
        {
            try
            {
                DirectoryInfo parent = Directory.GetParent(path);
                Directory.CreateDirectory(parent.FullName);

                using (StreamWriter writer = File.CreateText(path))
                {
                    await writer.WriteAsync(text);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to write file: " + path);
            }
        }
    }
}
