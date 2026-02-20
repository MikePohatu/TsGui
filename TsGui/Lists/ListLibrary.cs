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
using TsGui.Options;

namespace TsGui.Lists
{
    public static class ListLibrary
    {
        /// <summary>
        /// All files lists, including those owned by others
        /// </summary>
        private static Dictionary<string, FileList> AllFileLists { get; } = new Dictionary<string, FileList>();


        /// <summary>
        /// All Option Lists, including those owned by others
        /// </summary>
        private static Dictionary<string, OptionList> AllOptionLists { get; } = new Dictionary<string, OptionList>();

        /// <summary>
        /// Option Lists owned and processed by the OptionLibrary
        /// </summary>
        private static Dictionary<string, OptionList> OwnedOptionLists { get; } = new Dictionary<string, OptionList>();



        /// <summary>
        /// Get the OptionList for the ID. A new one will be created if it doesn't already exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OptionList GetOptionList(string id)
        {
            OptionList outlist;
            if (AllOptionLists.TryGetValue(id, out outlist) == false)
            {
                outlist = new OptionList(id, null);
                AllOptionLists.Add(id, outlist);
                OwnedOptionLists.Add(id, outlist);
            }
            return outlist;
        }

        /// <summary>
        /// Claim ownership of an OptionList i.e. remove it from ownership by the library. Will throw an 
        /// exception if List has already been claimed by another owner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OptionList ClaimOptionListOwnership(string id)
        {
            OptionList outlist;
            if (OwnedOptionLists.TryGetValue(id, out outlist))
            {
                OwnedOptionLists.Remove(id);
                return outlist;
            }

            if (AllOptionLists.TryGetValue(id, out outlist))
            {
                throw new KnownException($"Attempt to claim OptionList multiple times: {id}", "");
            }
            throw new KnownException($"OptionList not found: {id}", "");
        }

        public static List<Variable> GetVariables()
        {
            var variables = new List<Variable>();
            foreach (OptionList optList in OwnedOptionLists.Values)
            {
                variables.AddRange(optList.GetVariables());
            }

            return variables;
        }

        public static void LoadXml(XElement inputxml)
        {
            foreach (XElement listxml in inputxml.Elements("List"))
            {
                IList newlist = GetListFromXml(listxml, null);

            }
        }

        public static IList GetListFromXml(XElement inputxml, IVariableParent parent)
        {
            if (inputxml == null || inputxml.Name.ToString().Equals("List", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new KnownException("Invalid XML name passed to GetListFromXml function", inputxml.ToString());
            }

            string id = XmlHandler.GetStringFromXml(inputxml, "ID", null);
            if (id != null)
            {
                var list = new OptionList(id, parent);
                if (inputxml != null) { list.LoadXml(inputxml); }
                return list;
            }

            string file = XmlHandler.GetStringFromXml(inputxml, "File", null);
            if (file != null)
            {
                var list = new FileList(inputxml, parent);
                return list;
            }

            throw new KnownException("No valid list type found in XML, set a File or ID attribute", inputxml.ToString()); 
        }
        /// <summary>
        /// Clear the loaded lists
        /// </summary>
        public static void Reset()
        {
            AllOptionLists.Clear();
            OwnedOptionLists.Clear();
        }
    }
}
