//    Copyright (C) 2017 Mike Pohatu

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

// Conditional.cs - IF rule for queries

using System;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Validation;

namespace TsGui.Queries
{
    public class Conditional: IQuery
    {
        private StringMatchingRuleSet _processor = new StringMatchingRuleSet();
        private ResultWrangler _wrangler = new ResultWrangler();
        private ILinkingSource _sourceoption;
        private MainController _controller;
        private string _sourceID;

        public string TargetValue { get; set; }

        public Conditional(XElement inputxml, MainController controller)
        {
            this._controller = controller;
            this._controller.ConfigLoadFinished += this.OnControllerFinishedLoad;
            this.LoadXml(inputxml);
        }

        public void LoadXml(XElement InputXml)
        {
            //<IF>
            //  <SourceID>123</SourceID>
            //  <SourceValue>
            //      <Rule Type="StartsWith">test</Rule>
            //  </SourceValue>
            //  <TargetValue>
            //      <Query></Query>
            //      <Value></Value>
            //  </TargetValue>
            //</IF>
            this._sourceID = XmlHandler.GetStringFromXElement(InputXml, "SourceID", this._sourceID);
            this._wrangler.Separator = XmlHandler.GetStringFromXElement(InputXml, "Separator", this._wrangler.Separator);
            this._wrangler.IncludeNullValues = XmlHandler.GetBoolFromXElement(InputXml, "IncludeNullValues", this._wrangler.IncludeNullValues);
            this._wrangler.NewSubList();
            //this.SourceValue = XmlHandler.GetStringFromXElement(InputXml, "Value", this.SourceID);
        }

        public void OnSourceValueChanged(ILinkingSource source, EventArgs e)
        {

        }

        public void OnControllerFinishedLoad(object o, EventArgs e)
        { this._sourceoption = this._controller.LinkingLibrary.GetSourceOption(this._sourceID); }

        public ResultWrangler GetResultWrangler()
        {
            return this._wrangler;
        }

        public ResultWrangler ProcessQuery()
        { return this._wrangler; }
    }
}
