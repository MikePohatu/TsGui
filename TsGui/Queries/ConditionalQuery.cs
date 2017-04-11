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

// ConditionalQuery.cs - IF query

using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Validation.StringMatching;

namespace TsGui.Queries
{
    public class ConditionalQuery: BaseQuery, ILinkingEventHandler, ILinkTarget
    {
        private QueryList _sourcequerylist;
        private QueryList _resultquerylist;
        private RuleSet _ruleset = new RuleSet();
        private IDirector _controller;
        private ILinkTarget _linktargetoption;

        public ConditionalQuery(XElement inputxml, IDirector controller, ILinkTarget targetoption)
        {
            this._reprocess = true;
            this._controller = controller;
            this._sourcequerylist = new QueryList(this, this._controller);
            this._resultquerylist = new QueryList(this._controller);
            this._linktargetoption = targetoption;
            this.LoadXml(inputxml);
        }

        #region
        //<IF>
        //  <Source>
        //      <Query/>
        //  </Source>
        //  <Ruleset>
        //      <Rule Type="StartsWith">test</Rule>
        //  </Ruleset>
        //  <Result>
        //      <Query/>
        //  </Result>
        //</IF>
        #endregion
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;
            x = InputXml.Element("Source");
            if (x != null) { this._sourcequerylist.LoadXml(x); }

            x = InputXml.Element("Ruleset");
            if (x != null) { this._ruleset.LoadXml(x); }

            x = InputXml.Element("Result");
            if (x != null) { this._resultquerylist.LoadXml(x); }
        }

        public override ResultWrangler GetResultWrangler()
        {
            return this.ProcessQuery();
        }

        public override ResultWrangler ProcessQuery()
        {
            string sourcevalue = this._sourcequerylist.GetResultWrangler()?.GetString();

            if (this._ruleset.DoesMatch(sourcevalue) == true)
            { return this._resultquerylist.GetResultWrangler(); }
            else { return null; }
        }

        public void RefreshValue()
        {
            this._linktargetoption.RefreshValue();
        }

        public void OnLinkedSourceValueChanged()
        { this.RefreshValue(); }
    }
}
