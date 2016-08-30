//    Copyright (C) 2016 Mike Pohatu

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

// ResultWrangler.cs - deals with multiple results e.g. concatenating, filtering etc

using System.Collections.Generic;

namespace TsGui
{
    public class ResultWrangler
    {
        private List<List<ResultFormatter>> _results;
        private List<ResultFormatter> _currentList;

        /// <summary>
        /// Create a new List<ResultFormatter> and set it as current
        /// </summary>
        public void NewSubList()
        {
            if (this._results == null) { this._results = new List<List<ResultFormatter>>(); }
            List<ResultFormatter> newlist = new List<ResultFormatter>();
            this._results.Add(newlist);
            this._currentList = newlist;
        }

        /// <summary>
        /// Add a ResultFormatter to the ResultWrangler's current list 
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddResultFormatter(ResultFormatter Formatter)
        {
            this._currentList.Add(Formatter);
        }

        /// <summary>
        /// Add a ResultFormatter to the start ResultWrangler's current list 
        /// This will be used as the key when a dictionary is returned
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddKeyResultFormatter(ResultFormatter Formatter)
        {
            this._currentList.Insert(0,Formatter);
        }

        /// <summary>
        /// Concatenate ResultFormatter values into a single string
        /// </summary>
        /// <param name="Results"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public string ConcatenateResultValues(List<ResultFormatter> Results, string Separator)
        {
            string s = "";
            string tempval = null;
            int i = 0;

            foreach (ResultFormatter result in Results)
            {
                tempval = result.Value;
                if (!string.IsNullOrEmpty(tempval))
                {
                    if (i == 0) { s = tempval; }
                    else { s = s + Separator + tempval; }
                    i++;
                }
            }

            return s;
        }

        /// <summary>
        /// Get the _results list in dictionary format. First item in the list is the key, remainder is 
        /// concatenated with the separator
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> GetDictionary(string Separator)
        {
            Dictionary<string, string> returndic = new Dictionary<string, string>();

            foreach (List<ResultFormatter> sublist in this._results)
            {
                string concatList = ConcatenateResultValues(sublist.GetRange(1, sublist.Count - 1), Separator);
                string key = sublist[0].Value;

                returndic.Add(key, concatList);
            }

            return returndic;
        }
    }
}
