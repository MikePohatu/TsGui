#region license
// Copyright (c) 2025 Mike Pohatu
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

// TsTimeout.cs - Displays the value of the GUI timeout object

using MessageCrap;
using System.Threading.Tasks;
using System;
using System.Windows.Threading;
using System.Xml.Linq;
using TsGui.View.Layout;
using TsGui.Linking;
using System.Collections.Generic;

namespace TsGui.View.GuiOptions
{
    public class TsTimeout : GuiOptionBase, IGuiOption
    {
        private string _controltext;
        private bool _showelapsed = true;

        DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        //Properties

        //Custom stuff for control
        public override string CurrentValue { get { return this._controltext; } }
        public string ControlText
        {
            get { return this._controltext; }
            set { 
                this._controltext = value; 
                this.OnPropertyChanged(this, "ControlText");
                this.OnPropertyChanged(this, "CurrentValue");
            }
        }
        public override IEnumerable<Variable> Variables { get { return null; } }

        //constructor
        public TsTimeout(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this.ControlText = string.Empty;
            this.Control = new TsHeadingUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this.LoadXml(InputXml);
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            this._showelapsed = XmlHandler.GetBoolFromXml(InputXml, "ShowElapsed", this._showelapsed);
            if (this._showelapsed)
            {
                this._timer.Interval = TimeSpan.FromSeconds(1);
                this._timer.Tick += UpdateTimeout;
                this._timer.Start();
            }
            else
            {
                this.ControlText = GuiTimeout.Instance.AtString;
            }
        }

        public void UpdateTimeout(object sender, EventArgs e)
        {
            TimeSpan remaining = GuiTimeout.Instance.TimeLeft;
            if (remaining.Days > 0)
            {
                this.ControlText = remaining.ToString(@"dd\.hh\:mm\:ss");
            }
            else
            {
                this.ControlText = remaining.ToString(@"hh\:mm\:ss");
            }
        }

        public override async Task UpdateLinkedValueAsync(Message message)
        {
            LinkingHub.Instance.SendUpdateMessage(this, message);
            await Task.CompletedTask;
        }
    }
}
