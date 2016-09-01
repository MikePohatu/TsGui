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
        private Dictionary<int,List<ResultFormatter>> _lists;
        private Dictionary<int, ResultFormatter> _keys;
        private int _currentResultIndex;    //current index in the current list
        private int _currentResult;         //current result in the lists

        public string Separator { get; set; }
        public ResultWrangler()
        {
            this._lists = new Dictionary<int, List<ResultFormatter>>();
            this._keys = new Dictionary<int, ResultFormatter>();
            this.Separator = " '";
            this._currentResult = -1;
            this._currentResultIndex = 0;
        }


        /// <summary>
        /// Create a new List<ResultFormatter> and set it as current
        /// </summary>
        public void NewSubList()
        {
            this._currentResult++;
            this._currentResultIndex = 0;     //reset the current index

            List<ResultFormatter> newlist = new List<ResultFormatter>();
            this._lists.Add(this._currentResult, newlist);
        }

        /// <summary>
        /// Add a ResultFormatter to the ResultWrangler's current list 
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddResultFormatter(ResultFormatter Formatter)
        {
            if (this._currentResultIndex == 0) { this._keys.Add(this._currentResult, Formatter); }
            else
            {
                List<ResultFormatter> currentlist;
                this._lists.TryGetValue(this._currentResult, out currentlist);
                currentlist.Add(Formatter);
            }

            this._currentResultIndex++;
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
        /// concatenated with the specified separator
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public Dictionary<string,string> GetDictionary(string Separator)
        {
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentResult == -1) { return null; }

            Dictionary<string, string> returndic = new Dictionary<string, string>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i==this._currentResult; i++)
            {
                string concatlist = "";
                string value = "";

                if (this._keys.TryGetValue(i, out _tempRF))
                { value = _tempRF.Value; }
                
                if (this._lists.TryGetValue(i, out _tempRFList))
                { concatlist = this.ConcatenateResultValues(_tempRFList, Separator); }

                
                returndic.Add(value, concatlist);
            }

            return returndic;
        }

        /// <summary>
        /// Get the _results list in dictionary format. First item in the list is the key, remainder is 
        /// concatenated with the separator set in the wrangler object 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> GetDictionary()
        { return this.GetDictionary(this.Separator); }



        /// <summary>
        /// Get the _results list in a list of Key/Value pair format. First item in the list is the key, remainder is 
        /// concatenated with the separator set in the wrangler object 
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string,string>> GetKeyValueList()
        { return this.GetKeyValueList(this.Separator); }



        /// <summary>
        /// Get the _results list in a list of Key/Value pair format. First item in the list is the key, remainder is 
        /// concatenated with the specified separator
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetKeyValueList(string Separator)
        {
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentResult == -1) { return null; }

            List<KeyValuePair<string, string>> returnkvlist = new List<KeyValuePair<string, string>>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i == this._currentResult; i++)
            {
                string concatlist = "";
                string value = "";

                if (this._keys.TryGetValue(i, out _tempRF))
                { value = _tempRF.Value; }

                if (this._lists.TryGetValue(i, out _tempRFList))
                { concatlist = this.ConcatenateResultValues(_tempRFList, Separator); }


                returnkvlist.Add(new KeyValuePair<string,string>( value, concatlist));
            }

            return returnkvlist;
        }
    }
}
