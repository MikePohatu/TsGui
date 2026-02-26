#region license
// Copyright (c) 2025 Mike Pohatu
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
using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Xml.Serialization;
using TsGui.Options;

namespace TsGui.Lists
{
    public static class ListLibrary
    {
        /// <summary>
        /// All files lists, including those owned by others
        /// </summary>
        private static Dictionary<string, BaseList> AllLists { get; } = new Dictionary<string, BaseList>();

        /// <summary>
        /// File Lists owned and processed by the OptionLibrary
        /// </summary>
        private static Dictionary<string, BaseList> OwnedLists { get; } = new Dictionary<string, BaseList>();


        /// <summary>
        /// Claim ownership of an BaseList i.e. remove it from ownership by the library. Will throw an 
        /// exception if List has already been claimed by another owner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseList ClaimListOwnership(string id, IConfigParent parent)
        {
            BaseList outlist;
            if (OwnedLists.TryGetValue(id, out outlist))
            {
                OwnedLists.Remove(id);
                outlist.UpdateParent(parent);
                return outlist;
            }

            if (AllLists.TryGetValue(id, out outlist))
            {
                throw new KnownException($"Attempt to claim List multiple times: {id}", "");
            }
            throw new KnownException($"List not found: {id}", "");
        }

        /// <summary>
        /// Get a List by ID. Create a new default OptionList if doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OptionList GetOptionList(string id)
        {
            BaseList outlist = null;
            OptionList outOptList = null;
            if (AllLists.TryGetValue(id, out outlist))
            {
                outOptList = outlist as OptionList;
                if (outOptList == null)
                {
                    throw new KnownException($"List with ID '{id}' already exists but is not an OptionList", "");
                }
            }
            else
            {
                outOptList = new OptionList(id, null);
                AddList(outOptList);
            }

            return outOptList;
        }

        /// <summary>
        /// Get a List by File. Create a new default FileList if doesn't exist 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public static FileList GetFileList(string file)
        {
            BaseList outlist = null;
            FileList outFileList = null;
            if (AllLists.TryGetValue(file, out outlist))
            {
                outFileList = outlist as FileList;
                if (outFileList == null)
                {
                    throw new KnownException($"List with File '{file}' already exists but is not a FileList", "");
                }
            }
            else
            {
                outFileList = new FileList(file, null);
                AddList(outFileList);
            }

            return outFileList;
        }

        public async static Task<List<Variable>> ProcessAllAsync()
        {
            var variables = new List<Variable>();
            foreach (BaseList list in OwnedLists.Values)
            {
                var listvars = await list.ProcessAsync();
                variables.AddRange(listvars);
            }

            return variables;
        }

        public static void LoadXml(XElement inputxml)
        {
            foreach (XElement listxml in inputxml.Elements("List"))
            {
                BaseList newlist = LoadListFromXml(listxml);
            }
        }

        /// <summary>
        /// Add an OptionList to the Library
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="KnownException"></exception>
        private static void AddList(BaseList list)
        {
            BaseList outList;
            if (AllLists.TryGetValue(list.ID, out outList))
            {
                throw new KnownException($"Duplicate ID found adding List ID: {list.ID}","");
            }
            AllLists.Add(list.ID, list);
            OwnedLists.Add(list.ID, list);
        } 

        /// <summary>
        /// Factory method to get a List from an XML config. Load the XML for existing Lists
        /// </summary>
        /// <param name="inputxml"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public static BaseList LoadListFromXml(XElement inputxml)
        {
            if (inputxml == null || inputxml.Name.ToString().Equals("List", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new KnownException("Invalid XML name passed to GetListFromXml function", inputxml.ToString());
            }

            BaseList list = null;

            string file = XmlHandler.GetStringFromXml(inputxml, "File", null);
            string id = XmlHandler.GetStringFromXml(inputxml, "ID", null);

            if (file == null && id == null)
            { throw new KnownException("No valid list type found in XML, set a File or ID attribute", inputxml.ToString()); }

            if (file != null)
            {
                list = GetFileList(file);
            }
            else
            {
                list = GetOptionList(id);
            }

            list.LoadXml(inputxml);
            return list;
        }

        /// <summary>
        /// Clear the loaded lists
        /// </summary>
        public static void Reset()
        {
            AllLists.Clear();
            OwnedLists.Clear();
        }
    }
}
