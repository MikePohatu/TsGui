#region license
// Copyright (c) 2020 Mike Pohatu
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Diagnostics;

namespace TsGui.Config
{
    public class ConfigBuilder
    {
        private string _rootConfigPath;

        /// <summary>
        /// The current config 
        /// </summary>
        public XElement Config { get; private set; }

        /// <summary>
        /// New ConfigBuilder with no config set. You will need to use the load methods to load the config in
        /// </summary>
        public ConfigBuilder() { }

        /// <summary>
        /// New ConfigBuilder with the path to the root config file. Config will be loaded and any imports will be inlined
        /// </summary>
        /// <param name="configPath"></param>
        public ConfigBuilder(string configPath)
        {
            this._rootConfigPath = configPath;
        }

        /// <summary>
        /// Load the configured config. This will reset any existing config
        /// </summary>
        public async Task LoadConfig(string configpath)
        {
            this.Config = await this.GetXml(configpath);
            await this.Expand(this.Config);
        }

        /// <summary>
        /// Traverse the XML and expand any found Import elements in place
        /// </summary>
        /// <param name="importxml"></param>
        /// <returns></returns>
        private async Task Expand(XElement importxml)
        {
            foreach (XElement element in importxml.Elements())
            {
                if (element.Name == "Import")
                {
                    string path = XmlHandler.GetStringFromXElement(element, "Path", string.Empty);
                    path = XmlHandler.GetStringFromXAttribute(element, "Path", path);
                    XElement inx = await GetXml(path);

                    element.AddAfterSelf(inx);
                    element.Remove();
                } 
                else
                {
                    await this.Expand(element);
                }
            }
        }

        private async Task<XElement> GetXml(string configpath)
        {
            string lowerpath = configpath.ToLower();
            if (lowerpath.StartsWith("http://") || lowerpath.StartsWith("https://") || lowerpath.StartsWith("ftp://"))
            {
                return await XmlHandler.ReadWebAsync(configpath);
            }
            else
            {
                return await XmlHandler.ReadAsync(configpath);
            }
        }
    }
}
