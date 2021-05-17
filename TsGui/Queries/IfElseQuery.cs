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
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;

namespace TsGui.Queries
{
    public class IfElseQuery: BaseQuery
    {
        private List<Conditional> _conditions = new List<Conditional>();
        private QueryPriorityList _else;

        public IfElseQuery (XElement inputxml, ILinkTarget owner): base(owner)
        {
            this.LoadXml(inputxml);
        }

        public override async Task<ResultWrangler> GetResultWranglerAsync(Message message)
        {
            return await this.ProcessQueryAsync(message);
        }

        public override async Task<ResultWrangler> ProcessQueryAsync(Message message)
        {
            foreach (Conditional condition in this._conditions)
            {
                ResultWrangler returnval = await condition.GetResultWranglerAsync(message);
                if ((returnval != null) && (this.ShouldIgnore(returnval.GetString()) == false)) { return returnval; }
            }
            return await this._else?.GetResultWranglerAsync(message);
        }

        protected new void LoadXml(XElement inputxml)
        {
            base.LoadXml(inputxml);

            foreach (XElement element in inputxml.Elements())
            {
                string xname = element.Name.ToString();

                if (xname.Equals("IF",StringComparison.OrdinalIgnoreCase) == true)
                {
                    Conditional newcondition = new Conditional(element, this._linktarget);
                    this._conditions.Add(newcondition);
                }
                else if (xname.Equals("ELSE", StringComparison.OrdinalIgnoreCase) == true)
                {
                    this._else = new QueryPriorityList(element,this._linktarget);
                }
            }
        }
    }
}
