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
using Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TsGui.View.Layout.Events
{
    public enum EventDirection { Tunnel, Bubble }
    public delegate void LayoutEventHandler(object sender, LayoutEventArgs type);

    public class LayoutEvents
    {

        private IEventer _parent;
        private IEventer _source;

        private event LayoutEventHandler _tunnelEvent;
        private event LayoutEventHandler _bubbleEvent;

        private Dictionary<string, LayoutEventHandler> _subscriptions = new Dictionary<string, LayoutEventHandler>();
        /// <summary>
        /// Subscribe to a layout event topic, identified by a string. 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handler"></param>
        public void Subscribe(string topic, LayoutEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            LayoutEventHandler hand;
            if (this._subscriptions.TryGetValue(topic, out hand))
            {
                hand += handler;
            }
            else
            {
                this._subscriptions.Add(topic, handler);
            }
        }



        public LayoutEvents(IEventer source) { this._source = source; }
        public LayoutEvents(IEventer source, IEventer parent) 
        { 
            this._source = source; 
            this._parent = parent;
            if (this._parent != null)
            {
                this._parent.Events._tunnelEvent += this.OnLayoutEvent;
                this._bubbleEvent += this._parent.Events.OnLayoutEvent;
            }
        }

        private void OnLayoutEvent(object sender, LayoutEventArgs e)
        {
            //first check if there is a subsription for this event
            LayoutEventHandler hand;
            if (this._subscriptions.TryGetValue(e.Topic, out hand))
            {
                hand(sender, e);
            }

            var args = new LayoutEventArgs(this,e.OriginalSource,e.Topic, e.Direction);
            if (e.Direction == EventDirection.Bubble)
            {
                this._bubbleEvent?.Invoke(this, args);
            }
            else
            {
                this._tunnelEvent?.Invoke(this, args);
            }
        }

        public void InvokeLayoutEvent(string topic, EventDirection d)
        {
            var args = new LayoutEventArgs(this, this, topic, d);
            if (d == EventDirection.Bubble)
            {
                this._bubbleEvent?.Invoke(this, args);
            }
            else
            {
                this._tunnelEvent?.Invoke(this, args);
            }
        }
    }
}
