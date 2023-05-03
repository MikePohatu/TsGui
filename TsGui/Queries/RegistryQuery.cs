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
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Xml.Linq;
using TsGui.Connectors.System;
using Core.Diagnostics;
using TsGui.Linking;
using WindowsHelpers;
using System.Threading.Tasks;

namespace TsGui.Queries
{
    public class RegistryQuery : BaseQuery
    {
        private string _root = "HKEY_LOCAL_MACHINE";
        private string _value = string.Empty;
        private string _key = string.Empty;
        public RegistryQuery(ILinkTarget owner) : base(owner) { }

        public RegistryQuery(XElement InputXml, ILinkTarget owner) : base(owner)
        {
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this._processingwrangler.Separator = XmlHandler.GetStringFromXml(InputXml, "Separator", this._processingwrangler.Separator);
            this._processingwrangler.IncludeNullValues = XmlHandler.GetBoolFromXml(InputXml, "IncludeNullValues", this._processingwrangler.IncludeNullValues);
            this._root = XmlHandler.GetStringFromXml(InputXml, "Root", this._root);
            this._value = XmlHandler.GetStringFromXml(InputXml, "Value", this._value);
            this._key = XmlHandler.GetStringFromXml(InputXml, "Key", this._key);

            //validate the root 
            switch (this._root.ToUpper())
            {
                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                    this._root = "HKEY_LOCAL_MACHINE";
                    break;
                case "HKCU":
                case "HKEY_CURRENT_USER":
                    this._root = "HKEY_CURRENT_USER";
                    break;
                case "HKCR":
                case "HKEY_CLASSES_ROOT":
                    this._root = "HKEY_CLASSES_ROOT";
                    break;
                case "HKU":
                case "HKEY_USERS":
                    this._root = "HKEY_USERS";
                    break;
                default:
                    throw new KnownException("Invalid registry root name: " + this._root + Environment.NewLine + "See: https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry.getvalue?view=dotnet-plat-ext-5.0#remarks", null);
            }
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            //Query the reg value
            try
            {
                if (this._processed == true) { this._processingwrangler = this._processingwrangler.Clone(); }

                var results = RegistryHelpers.GetStringList($"{this._root}\\{this._key}", this._value);
                foreach (string result in results)
                {
                    FormattedProperty formatter = new FormattedProperty();
                    formatter.Input = result;
                    Result r = new Result();
                    r.KeyProperty = formatter;
                    this._processingwrangler.AddResult(r);
                }
            }
            catch (ManagementException e)
            {
                throw new KnownException("Registry query caused an error:" + Environment.NewLine + $"{this._root}\\{this._key}\\{this._value}", e.Message);
            }

            this._processed = true;
            if (this.ShouldIgnore(this._processingwrangler.GetString()) == false)
            { this._returnwrangler = this._processingwrangler; }
            else { this._returnwrangler = null; }

            await Task.CompletedTask;
            return this._returnwrangler;
        }
    }
}
