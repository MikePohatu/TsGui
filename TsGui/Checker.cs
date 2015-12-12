using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace TsGui
{
    public static class Checker
    {
        public static bool ShouldIgnore(XElement InputXml, string Value)
        {
            foreach (XElement xignore in InputXml.Elements("Ignore"))
            {
                //first check for empty value in the ignore entry i.e. it has been left in 
                //the file but has no use
                if (String.IsNullOrEmpty(xignore.Value.Trim()) == true) { continue; }

                bool toignore = false;
                XAttribute attrib = xignore.Attribute("SearchType");
                
                if (attrib != null)
                {
                    string s = xignore.Attribute("SearchType").Value;
                    //run the correct search type. default is startswith
                    if (s == "EndsWith")
                    { toignore = Value.EndsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                    else if (s == "Contains")
                    { toignore = Value.ToUpper().Contains(xignore.Value.ToUpper()); }
                    else if (s == "Equals")
                    { toignore = Value.Equals(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                    else
                    { toignore = Value.StartsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                }
                else
                { toignore = Value.StartsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }

                if (toignore == true) { return true; }
            }

            //match hasn't been found. Return false i.e. don't ignore
            return false;
        }

        public static string Truncate (string StringValue, int Length)
        {
            if (StringValue.Length > Length) { return StringValue.Substring(0, Length); }
            else { return StringValue; }
        }

        public static string RemoveInvalid (string StringValue, string InvalidChars)
        {
            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = InvalidChars.ToCharArray();
            string newstring = StringValue;

            foreach (char c in invalidchars)
            {
                newstring = newstring.Replace(c.ToString(),string.Empty);
            }

            return newstring;
        }
    }
}
