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

// GuiVariableQuery.cs - queries gui variables
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Options;
using TsGui.Linking;
using MessageCrap;
using System.Threading.Tasks;

namespace TsGui.Queries
{
    public class GuiVariableQuery: BaseQuery, ILinkTarget
    {
        private FormattedProperty _formatter;
        private List<IOption> _options = new List<IOption>();

        public GuiVariableQuery(XElement inputxml, ILinkTarget owner): base(owner)
        {         
            this.LoadXml(inputxml);
            this.AddExistingOptions();      //add and register any existing options
            OptionLibrary.OptionAdded += this.OnOptionAddedToLibrary; //register to get any new options
        }

        private void AddExistingOptions()
        {
            foreach (IOption option in OptionLibrary.Options)
            {
                if ((string.IsNullOrEmpty(option.VariableName) == false) && (option.VariableName.Equals(this._formatter.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    this.AddOption(option);
                     
                }
            }
        }

        //get the current variable value from the list of relevant options
        private string GetVariableValue()
        {
            if (!string.IsNullOrEmpty(this._formatter.Name))
            {
                string s = string.Empty;
                foreach (IOption opt in this._options)
                {
                    if (opt.IsActive == true) { s = opt.CurrentValue; }
                }
                return s;
            }
            else { return null; }
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        {
            await this.ValueUpdatedAsync(message);
        }

        public async Task ValueUpdatedAsync (Message message)
        {
            await this._linktarget?.OnSourceValueUpdatedAsync(message);
        }

        /// <summary>
        /// Process a <Query Type="GuiVariable"> block and return the ResultWrangler
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            this._formatter.Input = this.GetVariableValue();
            this._processed = true;
            await Task.CompletedTask;
            return this.SetReturnWrangler();         
        }

        private ResultWrangler SetReturnWrangler()
        {
            if (this._formatter.Input == null) { return null; }
            if (this.ShouldIgnore(this._formatter.Input) == true) { this._returnwrangler = null; }
            else { this._returnwrangler = this._processingwrangler; }
            return this._returnwrangler;
        }

        public void OnOptionAddedToLibrary(IOption option, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(option.VariableName)) && ( option.VariableName.Equals(this._formatter.Name,StringComparison.OrdinalIgnoreCase)))
            {
                this.AddOption(option);
            }
        }

        private void AddOption(IOption option)
        {
            this._options.Add(option);
        }

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;
            XAttribute xattrib;

            this._processingwrangler.NewResult();
            
            x = InputXml.Element("Variable");
            if (x != null)
            {
                //check for new xml syntax. If the name attribute doesn't exist, setup for the 
                //legacy layout.
                xattrib = x.Attribute("Name");
                if (xattrib == null)
                {
                    this._formatter = new FormattedProperty();
                    this._formatter.Name = x.Value;
                }
                else
                {
                    this._formatter = new FormattedProperty(x);
                }

                this._processingwrangler.AddFormattedProperty(this._formatter);
            }
        }
    }
}
