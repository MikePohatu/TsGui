#region license
// Copyright (c) 2026 Mike Pohatu
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
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Logging;

namespace TsGui.Config
{
    public static class ConfigBuilder
    {

        /// <summary>
        /// Load the configured config. This will reset any existing config
        /// </summary>
        public static async Task<XElement> LoadConfigAsync(string configpath)
        {
            int maxPasses = Arguments.Instance.ImportLevels;
            int pass = 0;
            int importCount = 0;
            XElement rootconfig =  await GetXml(configpath);

            // Expand any Import elements in the config. Do multiple passes to allow for nested imports
            do
            {
                importCount = await ExpandAsync(rootconfig);
                pass++;
            }
            while (importCount > 0 && pass < maxPasses);
            return rootconfig;
        }

        /// <summary>
        /// Traverse the XML and expand any found Import elements in place
        /// </summary>
        /// <param name="importxml"></param>
        /// <returns>int</returns>
        /// 
        private static async Task<int> ExpandAsync(XElement importxml)
        {
            int importCount = 0;
            var elenum = importxml.Elements().GetEnumerator();
            var hasnext = elenum.MoveNext();

            while (hasnext)
            {
                XElement element = elenum.Current;
                if (element.Name == "Import")
                {
                    importCount++;
                    string path = XmlHandler.GetStringFromXml(element, "Path", string.Empty);
                    path = XmlHandler.GetStringFromXml(element, "Path", path);
                    Log.Info("Loading XML import: " + element.ToString());

                    XElement partx = await GetXml(path);

                    element.AddBeforeSelf(new XComment(element.ToString()));

                    foreach (XElement partelement in partx.Elements())
                    {
                        element.AddBeforeSelf(partelement);
                    }

                    element.AddBeforeSelf(new XComment(element.ToString()));
                    hasnext = elenum.MoveNext();
                    element.Remove();
                } 
                else
                {
                    importCount += await ExpandAsync(element);
                    hasnext = elenum.MoveNext();
                }
            }

            return importCount;
        }

        /// <summary>
        /// Get XElement from uri, auto-detect web vs local based on path
        /// </summary>
        /// <param name="configpath"></param>
        /// <returns></returns>
        private static async Task<XElement> GetXml(string configpath)
        {
            string lowerpath = configpath.ToLower();
            XElement results = null;
            if (lowerpath.StartsWith("http://") || lowerpath.StartsWith("https://") || lowerpath.StartsWith("ftp://"))
            {
                results = await XmlHandler.ReadWebAsync(configpath);
            }
            else
            {
                results = XmlHandler.Read(configpath);
            }
            return results;
        }
    }
}
