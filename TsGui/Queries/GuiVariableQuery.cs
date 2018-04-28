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

// GuiVariableQuery.cs - queries gui variables
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Options;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class GuiVariableQuery: BaseQuery, ILinkingEventHandler
    {
        private FormattedProperty _formatter;
        private IDirector _director;
        private ILinkTarget _linktargetoption;
        private List<IOption> _options = new List<IOption>();

        public GuiVariableQuery(XElement inputxml, IDirector director, ILinkTarget owner)
        {
            this._linktargetoption = owner;
            this._director = director;            
            this.LoadXml(inputxml);
            this.AddExistingOptions();      //add and register any existing options
            this._director.OptionLibrary.OptionAdded += this.OnOptionAddedToLibrary; //register to get any new options
            this.ProcessAndRefresh();
        }

        private void AddExistingOptions()
        {
            foreach (IOption option in this._director.OptionLibrary.Options)
            {
                if ((!string.IsNullOrEmpty(option.VariableName)) && (option.VariableName.Equals(this._formatter.Name, StringComparison.OrdinalIgnoreCase)))
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

        private void ProcessAndRefresh()
        {
            this.ProcessQuery();
            this._linktargetoption?.RefreshValue();
        }

        public void OnLinkedSourceValueChanged()
        {
            this.ProcessAndRefresh();
        }

        /// <summary>
        /// Process a <Query Type="GuiVariable"> block and return the ResultWrangler
        /// </summary>
        /// <param name="InputXml"></param>
        /// <returns></returns>
        public override ResultWrangler ProcessQuery()
        {
            this._formatter.Input = this.GetVariableValue();
            this._processed = true;
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
                this.ProcessAndRefresh();
            }
        }

        private void AddOption(IOption option)
        {
            this._options.Add(option);
            option.ValueChanged += this.OnLinkedSourceValueChanged;
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
