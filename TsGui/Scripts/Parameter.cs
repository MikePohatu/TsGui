using Core.Diagnostics;
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.Scripts
{
    internal class Parameter
    {
        private QueryPriorityList _querylist;
        public string Name { get; set; }
        public bool IsSwitch { get; set; } = false;

        public async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            return await _querylist?.GetResultWrangler(message);
        }

        public Parameter(XElement InputXml, ILinkTarget owner)
        {
            this.LoadXml(InputXml, owner);
        }

        public void LoadXml(XElement InputXml, ILinkTarget owner)
        {
            if (InputXml.Name == "Switch") { this.IsSwitch = true; }

            XmlHandler.GetStringFromXAttribute(InputXml, "Name", this.Name);
            this.Name = XmlHandler.GetStringFromXElement(InputXml, "Name", this.Name);
            string value = XmlHandler.GetStringFromXAttribute(InputXml, "Value", null);

            //validation values
            if (string.IsNullOrEmpty(this.Name)) { throw new KnownException($"Parameter name not defined: {InputXml}", null); }

            //Values set as elements win over attributes
            XElement set = InputXml.Element("SetValue");
            if (set != null)
            {
                this._querylist = new QueryPriorityList(set, owner);
                if (this._querylist.Queries.Count == 0)
                {
                    if (!string.IsNullOrEmpty(value) && this.IsSwitch == false) { throw new KnownException($"Parameter {this.Name} does not define a value: {InputXml}", null); }
                    this._querylist = new QueryPriorityList(owner);
                    this._querylist.AddQuery(new ValueOnlyQuery(value));
                }
            }            
        }
    }
}
