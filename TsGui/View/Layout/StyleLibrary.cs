using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;
using Core.Logging;

namespace TsGui.View.Layout
{
    /// <summary>
    /// StyleLibrary stores and returns Style objects based on their ID value
    /// </summary>
    public static class StyleLibrary
    {
        private static Dictionary<string, StyleTree> _styles = new Dictionary<string, StyleTree>();

        /// <summary>
        /// Remove all styles for reload
        /// </summary>
        public static void Reset()
        {
            _styles.Clear();
        }

        public static void Add(StyleTree style)
        {
            if (string.IsNullOrEmpty(style.ID))
            {
                throw new KnownException("Style ID not specified", string.Empty);
            }

            if (_styles.ContainsKey(style.ID))
            {
                throw new KnownException("Duplicate Style ID found: " + style.ID, string.Empty);
            }

            _styles[style.ID] = style;
        }

        public static StyleTree Get(string id)
        {
            StyleTree outstyle;
            if (_styles.TryGetValue(id, out outstyle))
            {
                Log.Debug($"Style with ID {id} found");
                return outstyle;
            }
            else
            {
                Log.Error($"Style with ID {id} not found");
                return null;
            }
        }

        public static void LoadXml(XElement InputXml)
        {
            XElement stylesx = InputXml.Element("Styles");
            if (stylesx != null)
            {
                foreach (XElement x in stylesx.Elements("Style"))
                {
                    StyleTree s = new StyleTree(x);
                    Add(s);
                }
            }            
        }
    }
}
