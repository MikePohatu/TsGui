﻿#region license
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
using System.Xml.Linq;
using Core.Logging;
using TsGui.Linking;

namespace TsGui.Validation.StringMatching
{
    public abstract class BaseNumberMatchingRule : BaseMatchingRule
    {
        public BaseNumberMatchingRule(XElement inputxml, ILinkTarget linktarget) : base(inputxml, linktarget) { }

        public bool DoesMatch(string input)
        {
            double inputnum;
            double rulenum;

            if (!double.TryParse(input, out inputnum))
            {
                Log.Warn("Failed to convert input to number: " + input);
                return false;
            }
            if (!double.TryParse(this.Content, out rulenum))
            {
                Log.Warn("Failed to convert rule content to number: " + this.Content);
                return false;
            }

            bool result = this.Compare(inputnum, rulenum);
            if (this.Not) { return !result; }
            else { return result; }
        }

        protected abstract bool Compare(double input, double rulenumber);
    }
}
