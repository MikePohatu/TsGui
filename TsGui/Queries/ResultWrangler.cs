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

// ResultWrangler.cs - deals with multiple results e.g. concatenating, listing etc

using System.Collections.Generic;

namespace TsGui.Queries
{
    public class ResultWrangler
    {
        //two dictionaries, one for the keys i.e. the first value, and another for the values as a list. 
        //the two have matching keys to link the keys dictionary to the lists dictionary. 
        private Dictionary<int,List<ResultFormatter>> _valuelists = new Dictionary<int, List<ResultFormatter>>();
        private Dictionary<int, ResultFormatter> _keyvalues = new Dictionary<int, ResultFormatter>();
        private Dictionary<int, List<ResultWrangler>> _subwranglers = new Dictionary<int, List<ResultWrangler>>();

        //the following two variables track the lists and position in the lists.
        private int _currentresultlist;         //current result in the lists

        public string Separator { get; set; }
        public bool IncludeNullValues { get; set; }


        public ResultWrangler()
        {
            this.IncludeNullValues = true;
            this.Separator = ", ";
            this._currentresultlist = -1;
        }


        /// <summary>
        /// Create a new List<ResultFormatter> and set it as current
        /// </summary>
        public void NewResultList()
        {
            this._currentresultlist++;

            List<ResultFormatter> newlist = new List<ResultFormatter>();
            this._valuelists.Add(this._currentresultlist, newlist);
        }

        /// <summary>
        /// Add a ResultFormatter to the ResultWrangler's current list 
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddResultFormatter(ResultFormatter Formatter)
        {
            ResultFormatter keyresult;
            if (this._keyvalues.TryGetValue(this._currentresultlist, out keyresult) == false) { this._keyvalues.Add(this._currentresultlist, Formatter); }
            else
            {
                List<ResultFormatter> currentlist;
                this._valuelists.TryGetValue(this._currentresultlist, out currentlist);
                currentlist.Add(Formatter);
            }
        }

        public void AddResultFormatters(List<ResultFormatter> Formatters)
        {
            foreach (ResultFormatter rf in Formatters)
            { this.AddResultFormatter(rf); }
        }

        public void AddSubWrangler(ResultWrangler wrangler)
        {
            List<ResultWrangler> wranglerlist;
            if (this._subwranglers.TryGetValue(this._currentresultlist,out wranglerlist) == true) { wranglerlist.Add(wrangler); }
            else
            {
                wranglerlist = new List<ResultWrangler>();
                wranglerlist.Add(wrangler);
                this._subwranglers.Add(this._currentresultlist, wranglerlist);
            }
        }

        public List<ResultFormatter> GetAllResultFormatters()
        {
            List<ResultFormatter> formatterlist = new List<ResultFormatter>();
            foreach (int i in this._keyvalues.Keys)
            {
                ResultFormatter rf;
                if (this._keyvalues.TryGetValue(i, out rf) == true)
                { formatterlist.Add(rf); }

                List<ResultFormatter> rfs;
                if (this._valuelists.TryGetValue(i, out rfs) == true)
                { formatterlist.AddRange(rfs); }
            }

            return formatterlist;
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

                if (this.IncludeNullValues || !string.IsNullOrEmpty(tempval))
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

        public new string ToString() { return this.GetString(); }
        /// <summary>
        /// Get the _results list in a list of Key/Value pair format. First item in the list is the key, remainder is 
        /// concatenated with the specified separator
        /// </summary>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetKeyValueList(string Separator)
        {
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentresultlist == -1) { return null; }

            List<KeyValuePair<string, string>> returnkvlist = new List<KeyValuePair<string, string>>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentresultlist; i++)
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
            if (_currentresultlist == -1) { return null; }

            Dictionary<string, string> returndic = new Dictionary<string, string>();
            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentresultlist; i++)
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
            if (_currentresultlist == -1) { return null; }

            ResultFormatter _tempRF;
            List<ResultFormatter> _tempRFListMain = new List<ResultFormatter>();
            List<ResultFormatter> _tempRFList;

            for (int i = 0; i <= this._currentresultlist; i++)
            {

                if (this._keyvalues.TryGetValue(i, out _tempRF))
                { _tempRFListMain.Add(_tempRF); }

                if (this._valuelists.TryGetValue(i, out _tempRFList))
                { _tempRFListMain.AddRange(_tempRFList); }

                if (i == 0) { s = this.ConcatenateResultValues(_tempRFListMain, Separator); }
                else { s = this.ConcatenateResultValues(_tempRFListMain, Separator); }
            }

            return s;
        }

        /// <summary>
        /// Create a new ResultWrangler, using this as a template. Copy separator and IncludeNullValues
        /// </summary>
        /// <returns></returns>
        public ResultWrangler Clone()
        {
            ResultWrangler newwrangler = new ResultWrangler();
            newwrangler.Separator = this.Separator;
            newwrangler.IncludeNullValues = this.IncludeNullValues;
            return newwrangler;
        }
    }
}
