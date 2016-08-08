//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// TsHeading.cs - Label with no control. Used to add headings and text only 
// amongst the other options. 

using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public class TsHeading: TsBaseOption,IGuiOption
    {
        new private Label _control;
        private bool _bold;

        public TsHeading(XElement SourceXml, MainController RootController) : base()
        {
            this._controller = RootController;

            this._control = new Label();
            base._control = this._control;
            
            this.LoadXml(SourceXml);
            this.Build();
        }

        public TsVariable Variable { get { return null; } }

        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;
            this.LoadBaseXml(InputXml);

            x = InputXml.Element("Bold");
            if (x != null)
            { this._bold = true; }

            #endregion
        }

        private void Build()
        {
            //this._control = new Label();
            this._control.Content = "";
            if (this._bold) { this._labelcontrol.FontWeight = FontWeights.Bold; }
        }
    }
}
