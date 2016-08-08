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

// Group.cs - groups of elements to be enabled and disabled by a toggle

using System.Collections.Generic;
using System;
//using System.Diagnostics;

namespace TsGui
{
    public class Group
    {
        private List<IGroupable> _elements;
        private bool _isEnabled;
        private bool _isHidden;
        //private Guid _guid = Guid.NewGuid();

        //public Toggle Toggle { get; set; }
        public string ID { get; set; }
        public int Count { get { return this._elements.Count; } }
        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set
            {
                this._isEnabled = value;
                foreach (IGroupable element in this._elements)
                {
                    element.IsEnabled = value;
                }
            }
        }
        public bool IsHidden
        {
            get { return this._isHidden; }
            set
            {
                //Debug.WriteLine("Group Hidden set: " + this.ID + " - " + value);
                this._isHidden = value;
                foreach (IGroupable element in this._elements)
                {                    
                    element.IsHidden = value;
                }
            }
        }
        public Group (string ID)
        {
            this._elements = new List<IGroupable>();
            this.ID = ID;
            this.IsEnabled = true;
            this.IsHidden = false;
        }

        public void Add(IGroupable GroupableElement)
        {
            //Debug.WriteLine("Adding element to group: " + this.ID);
            //Debug.WriteLine(" Group guid: " + this._guid.ToString());
            this._elements.Add(GroupableElement);
        }

        public void Remove(IGroupable GroupableElement)
        {
            this._elements.Remove(GroupableElement);
        }
    }
}
