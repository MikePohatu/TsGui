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

using Core.Logging;
using MessageCrap;
using System.Threading.Tasks;
using TsGui.Grouping;
using TsGui.Linking;

namespace TsGui.Options
{
    /// <summary>
    /// Option class for things not configured by config.xml e.g.
    /// options controlled by authention activities. 
    /// </summary>
    public class MiscOption: GroupableBlindBase, IOption
    {
        private string _value = string.Empty;

        //properties
        public bool IsToggle { get; set; }
        public string Path { get; set; }
        public string ID { get; set; }
        public string VariableName { get; set; }
        public string InactiveValue { get; set; } = "TSGUI_INACTIVE";
        public string CurrentValue
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.NotifyViewUpdate();
            }
        }
        public Variable Variable
        {
            get { return new Variable(this.VariableName, this._value, this.Path); }
        }
        public string LiveValue
        {
            get { return this.CurrentValue; }
        }
        //constructors     
        public MiscOption(string variablename, string value) : base()
        {
            this.Path = Director.Instance.DefaultPath;
            this.CurrentValue = value;
            this.VariableName = variablename;
            this.NotifyViewUpdate();
        }

        public MiscOption() : base()
        {
            this.Path = Director.Instance.DefaultPath;
            this.NotifyViewUpdate();
        }

        //public methods
        public async Task UpdateValueAsync(Message message)
        {
            await Task.CompletedTask;
            LinkingHub.Instance.SendUpdateMessage(this, message);
            this.NotifyViewUpdate();
        }

        public async Task OnSourceValueUpdatedAsync(Message message)
        {
            await this.UpdateValueAsync(message);
        }

        protected void NotifyViewUpdate()
        {
            Log.Info(this.VariableName + " variable value changed. New value: " + this.LiveValue);
            this.OnPropertyChanged(this, "CurrentValue");
            this.OnPropertyChanged(this, "LiveValue");
        }

        //This is called by the controller once everything is loaded
        public async Task InitialiseAsync()
        {
            await this.UpdateValueAsync(null);
        }
    }
}
