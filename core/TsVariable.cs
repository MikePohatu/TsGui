using System;

namespace core
{
    public class TsVariable
    {
        public string Value { get; set; }
        public string Name 
        { 
            get; 
            set
            {
                if (value == null) { throw new InvalidOperationException("TsVariable name cannot be null"); }
                else { this.Name = value; }
            }
        }

        public TsVariable (string pName, string pValue)
        {
            this.Name = pName;
            this.Value = pValue;
        }
    }
}
