using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;

namespace TsGui
{
    public static class Checker
    {
        /// <summary>
        /// Check xml value against ignore strings. Return true if the value should be ignored
        /// </summary>
        /// <param name="InputXml"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool ShouldIgnore(XElement InputXml, string Value)
        {
            foreach (XElement xignore in InputXml.Elements("Ignore"))
            {
                //first check for empty value in the ignore entry i.e. it has been left in 
                //the file but has no use
                if (string.IsNullOrEmpty(xignore.Value.Trim())) { continue; }

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

                Debug.WriteLine("toignore value: " + toignore);
                if (toignore) { return true; }
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
            if (string.IsNullOrEmpty(InvalidChars)) { return StringValue; }
            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = InvalidChars.ToCharArray();
            string newstring = StringValue;

            foreach (char c in invalidchars)
            {
                newstring = newstring.Replace(c.ToString(),string.Empty);
            }

            return newstring;
        }

        public static bool ValidCharacters( string StringValue, string InvalidChars, bool CaseSensitive)
        {
            if (string.IsNullOrEmpty(InvalidChars)) { return true; }

            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = InvalidChars.ToCharArray();

            foreach (char c in invalidchars)
            {
                if (CaseSensitive == true)
                {
                    if (StringValue.ToUpper().Contains(c.ToString().ToUpper())) { return false; }
                }
                else
                {
                    if (StringValue.Contains(c.ToString())) { return false; }
                }             
            }

            return true;
        }


        public static bool ValidMaxLength(string StringValue,int MaxLength)
        {
            //if max length is 0, maxlength is undefined 
            if (MaxLength == 0 ) { return true; }
            if (StringValue.Length <= MaxLength) { return true; }
            else { return false; }
        }


        public static bool ValidMinLength(string StringValue, int MinLength)
        {
            //Debug.WriteLine(StringValue.Length);
            if (StringValue.Length >= MinLength) { return true; }
            else { return false; }
        } 
    }
}
