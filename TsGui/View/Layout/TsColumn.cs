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

// TsColumn.cs - class for columns in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

using TsGui.View.Layout;
using TsGui.View.GuiOptions;

namespace TsGui
{
    public class TsColumn : ParentLayoutElement
    {
        private List<IGuiOption> _options = new List<IGuiOption>();
        private Grid _columnpanel;
        private int _rowindex = 0;

        //properties
        #region
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this._options; } }
        public Grid Panel { get { return this._columnpanel; } }
        
        #endregion

        //constructor
        #region
        public TsColumn (XElement SourceXml,int PageIndex, ParentLayoutElement Parent) :base (Parent)
        {
            this.Index = PageIndex;
            this._columnpanel = new Grid();
            this._columnpanel.Name = "_columnpanel";
            this._columnpanel.DataContext = this;
            this._columnpanel.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._columnpanel.SetBinding(Grid.VisibilityProperty, new Binding("Visibility"));
            this._columnpanel.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this._columnpanel.SetBinding(Grid.WidthProperty, new Binding("Style.Width"));

            this.LoadXml(SourceXml);
        }
        #endregion

        private new void LoadXml(XElement InputXml)
        {
            this.Style.Width = XmlHandler.GetDoubleFromXml(InputXml, "Width", double.NaN);
            base.LoadXml(InputXml);
            this.LoadXml(InputXml, this);
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            IGuiOption newOption;

            //now read in the options and add to a dictionary for later use
            //do this last so the event subscriptions don't get setup too early (no toggles fired 
            //until everything is loaded.
            foreach (XElement x in InputXml.Elements())
            {
                if (x.Name == "GuiOption")
                {
                    newOption = GuiFactory.CreateGuiOption(x, parent);
                    if (newOption == null) { continue; }
                    this._options.Add(newOption);

                    RowDefinition rowdef = new RowDefinition();
                    rowdef.Height = GridLength.Auto;
                    this._columnpanel.RowDefinitions.Add(rowdef);
                    Grid.SetRow(newOption.UserControl, this._rowindex);

                    this._columnpanel.Children.Add(newOption.UserControl);

                    this._rowindex++;
                }
                else if (x.Name == "Container")
                {
                    new UIContainer(this, x);
                }
            }
        }
    }
}
