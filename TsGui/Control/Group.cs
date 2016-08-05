using System.Collections.Generic;

namespace TsGui
{
    public class Group
    {
        private List<IGroupable> _elements;
        private bool _isEnabled;
        private bool _isHidden;
        public string ID { get; set; }
        public bool Enabled
        {
            get { return this._isEnabled; }
            set
            {
                this._isEnabled = value;
                foreach (IGroupable element in this._elements)
                {
                    element.Enabled = value;
                }
            }
        }
        public bool Hidden
        {
            get { return this._isHidden; }
            set
            {
                this._isHidden = value;
                foreach (IGroupable element in this._elements)
                {
                    element.Hidden = value;
                }
            }
        }

        public Group (string ID)
        {
            this._elements = new List<IGroupable>();
            this.ID = ID;
            this.Enabled = true;
            this.Hidden = false;
        }

        public void Add (IGroupable GroupableElement)
        {
            this._elements.Add(GroupableElement);
        }

        public void Remove(IGroupable GroupableElement)
        {
            this._elements.Remove(GroupableElement);
        }

    }
}
