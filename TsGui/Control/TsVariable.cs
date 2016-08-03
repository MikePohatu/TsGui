using System;

namespace TsGui
{
    public class TsVariable
    {
        private string _name;
        private string _value;

        public string Value
        {
            get { return this._value; }
            set
            {
                if (value == null) { this._value = string.Empty; }
                else { this._value = value; }
            }
        }
        public string Name 
        {
            get { return this._name; } 
            set
            {
                if (value == null) { throw new InvalidOperationException("TsVariable name cannot be null"); }
                else { this._name = value; }
            }
        }

        public TsVariable (string Name, string Value)
        {
            this._name = Name;
            this.Value = Value;
        }
    }
}
