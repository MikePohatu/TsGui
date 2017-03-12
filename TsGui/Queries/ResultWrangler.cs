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

namespace TsGui.Queries
{
    public class ResultWrangler
    {
        //two dictionaries, one for the keys i.e. the first value, and another for the values as a list. 
        //the two have matching keys to link the keys dictionary to the lists dictionary. 
        private Dictionary<int,List<ResultFormatter>> _valuelists;
        private Dictionary<int, ResultFormatter> _keyvalues;

        //the following two variables track the lists and position in the lists.
        private int _currentIndex;    //current index in the current list
        private int _currentValue;         //current result in the lists

        public string Separator { get; set; }


        public ResultWrangler()
        {
            this._valuelists = new Dictionary<int, List<ResultFormatter>>();
            this._keyvalues = new Dictionary<int, ResultFormatter>();
            this.Separator = ", ";
            this._currentValue = -1;
            this._currentIndex = 0;
        }


        /// <summary>
        /// Create a new List<ResultFormatter> and set it as current
        /// </summary>
        public void NewSubList()
        {
            this._currentValue++;
            this._currentIndex = 0;     //reset the current index

            List<ResultFormatter> newlist = new List<ResultFormatter>();
            this._valuelists.Add(this._currentValue, newlist);
        }

        /// <summary>
        /// Add a ResultFormatter to the ResultWrangler's current list 
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddResultFormatter(ResultFormatter Formatter)
        {
            if (this._currentIndex == 0) { this._keyvalues.Add(this._currentValue, Formatter); }
            else
            {
                List<ResultFormatter> currentlist;
                this._valuelists.TryGetValue(this._currentValue, out currentlist);
                currentlist.Add(Formatter);
            }

            this._currentIndex++;
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


        public string GetString()
        { return this.GetString(this.Separator); }

        /// <summary>
        /// Get the _results list in a list of Key/Value pair format. First item in the list is the key, remainder is 
        /// concatenated with the specified separator
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetKeyValueList(string Separator)
        {
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentValue == -1) { return null; }

            List<KeyValuePair<string, string>> returnkvlist = new List<KeyValuePair<string, string>>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentValue; i++)
            {
                string concatlist = "";
                string value = "";

                if (this._keyvalues.TryGetValue(i, out _tempRF))
                { value = _tempRF.Value; }

                if (this._valuelists.TryGetValue(i, out _tempRFList))
                { concatlist = this.ConcatenateResultValues(_tempRFList, Separator); }

                returnkvlist.Add(new KeyValuePair<string,string>( value, concatlist));
            }

            return returnkvlist;
        }

        /// <summary>
        /// Get the _results list in dictionary format. First item in the list is the key, remainder is 
        /// concatenated with the specified separator
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionary(string Separator)
        {
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentValue == -1) { return null; }

            Dictionary<string, string> returndic = new Dictionary<string, string>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentValue; i++)
            {
                string concatlist = "";
                string value = "";

                if (this._keyvalues.TryGetValue(i, out _tempRF))
                { value = _tempRF.Value; }

                if (this._valuelists.TryGetValue(i, out _tempRFList))
                { concatlist = this.ConcatenateResultValues(_tempRFList, Separator); }

                returndic.Add(value, concatlist);
            }

            return returndic;
        }

        /// <summary>
        /// Get the _results list as a single string
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public string GetString(string Separator)
        {
            string s = null;

            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentValue == -1) { return null; }

            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFListMain = new List<ResultFormatter>();
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentValue; i++)
            {

                if (this._keyvalues.TryGetValue(i, out _tempRF))
                { _tempRFListMain.Add(_tempRF); }

                if (this._valuelists.TryGetValue(i, out _tempRFList))
                { _tempRFListMain.AddRange(_tempRFList); }

                if (i == 0) { s = this.ConcatenateResultValues(_tempRFListMain, Separator); }
                else { s = s + Separator + this.ConcatenateResultValues(_tempRFListMain, Separator); }
            }

            return s;
        }
    }
}
