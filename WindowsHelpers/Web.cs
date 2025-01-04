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
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsHelpers
{
    public static class Web
    {
        /// <summary>
        /// Get a text file and parse to a string.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public static async Task<string> ReadStringAsync(string url)
        {
            string responseBody = null;

            using (HttpClient _client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    throw new KnownException("Error downloading web config: " + url, e.Message);
                }
            }


            return responseBody;
        }
    }
}
