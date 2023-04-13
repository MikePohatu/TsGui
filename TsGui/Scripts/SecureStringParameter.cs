using Core.Diagnostics;
using MessageCrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TsGui.Authentication;
using TsGui.Linking;
using TsGui.Queries;

namespace TsGui.Scripts
{
    public class SecureStringParameter : IParameter
    {
        private IPassword _passwordSource;

        public string ID { get; private set; }
        public string Name { get; set; }

        public SecureStringParameter(XElement InputXml)
        {
            this.LoadXml(InputXml);
            Director.Instance.ConfigLoadFinished += this.OnConfigLoadFinished;
        }

        public async Task<object> GetValue(Message message)
        {
            await Task.CompletedTask;
            return this._passwordSource?.SecureString;
        }

        public void LoadXml(XElement InputXml)
        {
            this.ID = XmlHandler.GetStringFromXAttribute(InputXml, "SourceID", this.ID);
            this.Name = XmlHandler.GetStringFromXAttribute(InputXml, "Name", this.Name);
            this.Name = XmlHandler.GetStringFromXElement(InputXml, "Name", this.Name);

            //validation values
            if (string.IsNullOrEmpty(this.Name)) { throw new KnownException($"Parameter name not defined:\n{InputXml}", null); }

        }

        //do nothing on source value updated. These are triggered by the script running and querying
        //the parameter
        public async Task OnSourceValueUpdatedAsync(Message message) 
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// When the full config load has finished, get the source password from the AuthLibrary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="KnownException"></exception>
        public void OnConfigLoadFinished(object sender, EventArgs e)
        {
            var source = LinkingHub.Instance.GetSourceOption(this.ID) as IPassword;
            if (source == null) { throw new KnownException($"ID '{this.ID}' not found for SecureString parameter", null); }
            this._passwordSource = source;
        }
    }
}
