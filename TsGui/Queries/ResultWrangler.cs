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

// ResultWrangler.cs - takes a list of results, and outputs various formats 
// e.g. concatenating, listing, key-value pairs etc

using System.Collections.Generic;
using TsGui.Queries.Trees;

namespace TsGui.Queries
{
    public class ResultWrangler
    {
        private Dictionary<int, Result> _results = new Dictionary<int, Result>();

        private int _currentresultlist = -1;         //current working result in the dictionary

        public string Separator { get; set; } = ", ";
        public bool IncludeNullValues { get; set; } = true;

        public List<Result> GetResults()
        {
            List<Result> rlist = new List<Result>();
            foreach (Result r in this._results.Values)
            { rlist.Add(r); }
            return rlist;
        }

        /// <summary>
        /// Create a new List<FormattedProperty> and set it as current
        /// </summary>
        public void NewResult()
        {
            this._currentresultlist++;
            this._results.Add(this._currentresultlist, new Result());
        }

        public void AddResult(Result r)
        {
            if (r != null)
            {
                this._currentresultlist++;
                this._results.Add(this._currentresultlist, r);
            }
        }

        /// <summary>
        /// Add a PropertyFormatter to the ResultWrangler's current list 
        /// </summary>
        /// <param name="Formatter"></param>
        public void AddFormattedProperty(FormattedProperty Formatter)
        {
            Result result;
            this._results.TryGetValue(this._currentresultlist, out result);
            result.Add(Formatter); 
        }

        public void AddFormattedProperties(List<FormattedProperty> Formatters)
        {
            foreach (FormattedProperty rf in Formatters)
            { this.AddFormattedProperty(rf); }
        }

        public List<FormattedProperty> GetAllPropertyFormatters()
        {
            List<FormattedProperty> l = new List<FormattedProperty>();
            foreach (Result r in this._results.Values)
            {
                l.AddRange(r.GetAllFormattedProperties());
            }

            return l;
        }

        public List<KeyValueTreeNode> GetKeyValueTree()
        {
            if (_currentresultlist == -1) { return null; }
            List<KeyValueTreeNode> tree = new List<KeyValueTreeNode>();

            foreach (Result r in this._results.Values)
            {
                tree.Add(CreateKeyValueTreeNode(r));
            }

            return tree;
        }

        private KeyValueTreeNode CreateKeyValueTreeNode(Result result)
        {
            KeyValueTreeNode newnode = new KeyValueTreeNode();
            string value = result.KeyProperty.Value;
            string concatlist = this.ConcatenatePropertyValues(result.Properties, Separator);

            newnode.Value = new KeyValuePair<string, string>(value, concatlist);
            foreach (Result subresult in result.SubResults)
            {
                newnode.Nodes.Add(CreateKeyValueTreeNode(subresult));
            }
            return newnode;
        } 

        /// <summary>
        /// Concatenate FormattedProperty values into a single string
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public string ConcatenatePropertyValues(List<FormattedProperty> properties, string Separator)
        {
            string s = "";
            string tempval = null;
            int i = 0;

            foreach (FormattedProperty property in properties)
            {
                tempval = property.Value;

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

            foreach (Result r in this._results.Values)
            {
                string value = r.KeyProperty.Value;
                string concatlist = this.ConcatenatePropertyValues(r.Properties, Separator);

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

            foreach (Result r in this._results.Values)
            {
                string value = r.KeyProperty.Value;
                string concatlist = this.ConcatenatePropertyValues(r.Properties, Separator); 

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
            //first check to make sure a new sublist has actually been created. if not reutrn null
            if (_currentresultlist == -1) { return null; }

            List<FormattedProperty> tempRFListMain = new List<FormattedProperty>();

            foreach (Result r in this._results.Values)
            {
                tempRFListMain.Add(r.KeyProperty);
                tempRFListMain.AddRange(r.Properties);
            }

            return this.ConcatenatePropertyValues(tempRFListMain, Separator);
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
