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

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class IfElseQuery: BaseQuery, ILinkingEventHandler, ILinkTarget
    {
        private List<Conditional> _conditions = new List<Conditional>();
        private QueryPriorityList _else;
        private IDirector _director;
        private ILinkTarget _targetoption;

        public IfElseQuery (XElement inputxml, IDirector director, ILinkTarget targetoption)
        {
            this._director = director;
            this._targetoption = targetoption;
            this.LoadXml(inputxml);
        }

        public override ResultWrangler GetResultWrangler()
        {
            return this.ProcessQuery();
        }

        public override ResultWrangler ProcessQuery()
        {
            foreach (Conditional condition in this._conditions)
            {
                ResultWrangler returnval = condition.GetResultWrangler();
                if ((returnval != null) && (this.ShouldIgnore(returnval.GetString()) == false)) { return returnval; }
            }
            return this._else?.GetResultWrangler();
        }

        public void RefreshValue()
        {
            this._targetoption.RefreshValue();
        }

        public void RefreshAll()
        {
            this._targetoption.RefreshAll();
        }

        public void OnLinkedSourceValueChanged()
        { this.RefreshValue(); }

        protected new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);

            foreach (XElement element in inputxml.Elements())
            {
                string xname = element.Name.ToString();

                if (xname.Equals("IF",StringComparison.OrdinalIgnoreCase) == true)
                {
                    Conditional newcondition = new Conditional(element, this._director, this._targetoption);
                    this._conditions.Add(newcondition);
                }
                else if (xname.Equals("ELSE", StringComparison.OrdinalIgnoreCase) == true)
                {
                    this._else = new QueryPriorityList(element,this._targetoption,this._director);
                }
            }
        }
    }
}
