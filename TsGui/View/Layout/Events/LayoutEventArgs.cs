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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.View.Layout.Events
{
    /// <summary>
    /// Arguments for LayoutEvent
    /// </summary>
    public class LayoutEventArgs: EventArgs
    {
        public LayoutEvents Source { get; set; }
        public LayoutEvents OriginalSource { get; set; }
        public string Topic { get; set; }
        public EventDirection Direction { get; set; }

        public LayoutEventArgs(LayoutEvents source, LayoutEvents originalSource, string topic, EventDirection direction): base()
        {
            Source = source;
            OriginalSource = originalSource;
            Direction = direction;
            Topic = topic;
        }

        public LayoutEventArgs(LayoutEvents source, string topic, EventDirection direction) : base()
        {
            Source = source;
            Direction = direction;
            Topic = topic;
        }
    }
}
