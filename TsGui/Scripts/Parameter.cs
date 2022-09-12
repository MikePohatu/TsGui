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
    internal class Parameter: ILinkTarget
    {
        private QueryPriorityList _querylist;
        public string Name { get; set; }
        public bool IsSwitch { get; set; } = false;

        public Parameter(XElement InputXml)
        {
            this.LoadXml(InputXml);
        }

        public async Task<ResultWrangler> GetResultWrangler(Message message)
        {
            return await _querylist?.GetResultWrangler(message);
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml.Name == "Switch") { this.IsSwitch = true; }

            this.Name = XmlHandler.GetStringFromXAttribute(InputXml, "Name", this.Name);
            this.Name = XmlHandler.GetStringFromXElement(InputXml, "Name", this.Name);
            string value = XmlHandler.GetStringFromXAttribute(InputXml, "Value", null);

            //validation values
            if (string.IsNullOrEmpty(this.Name)) { throw new KnownException($"Parameter name not defined:\n{InputXml}", null); }

            //Values set as elements win over attributes
            XElement set = InputXml.Element("SetValue");
            if (set != null)
            {
                this._querylist = new QueryPriorityList(set, this);
                if (this._querylist.Queries.Count == 0)
                {
                    if (string.IsNullOrEmpty(value) && this.IsSwitch == false) { throw new KnownException($"Parameter {this.Name} does not define a value:\n{InputXml}", null); }
                    this._querylist = new QueryPriorityList(this);
                    this._querylist.AddQuery(new ValueOnlyQuery(value));
                }
            }            
        }

        //do nothing on source value updated. These are triggered by the script running and querying
        //the parameter
        public async Task OnSourceValueUpdatedAsync(Message message) 
        {
            await Task.CompletedTask;
        }
    }
}
