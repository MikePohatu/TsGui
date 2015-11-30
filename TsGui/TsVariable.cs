using System;

namespace TsGui
{
    public class TsVariable
    {
        private string name;
        public string Value { get; set; }
        public string Name 
        {
            get { return this.name; } 
            set
            {
                if (value == null) { throw new InvalidOperationException("TsVariable name cannot be null"); }
                else { this.name = value; }
            }
        }

        public TsVariable (string pName, string pValue)
        {
            this.name = pName;
            this.Value = pValue;
        }
    }
}
