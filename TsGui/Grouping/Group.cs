using System.Collections.Generic;
using System.Diagnostics;

namespace TsGui
{
    public class Group
    {
        private List<IGroupable> _elements;
        private bool _isEnabled;
        private bool _isHidden;

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
                Debug.WriteLine("Group Hidden set: " + this.ID + " - " + value);
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
            this._elements.Add(GroupableElement);
        }

        public void Remove(IGroupable GroupableElement)
        {
            this._elements.Remove(GroupableElement);
        }
    }
}
