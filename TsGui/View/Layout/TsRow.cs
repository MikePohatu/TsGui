﻿#region license
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

// TsRow.cs - class for rows in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsRow : ParentLayoutElement
    {
        private Grid _rowpanel;
        private List<TsColumn> _columns = new List<TsColumn>();
        private List<IGuiOption> _options = new List<IGuiOption>();

        //properties
        #region
        public List<IGuiOption> Options { get { return this._options; } }
        public int Index { get; set; }
        public Grid Panel { get { return this._rowpanel; } }
        #endregion

        //constructor
        #region
        public TsRow (XElement SourceXml,int PageIndex, ParentLayoutElement Parent): base (Parent)
        {
            this.Index = PageIndex;
            this._rowpanel = new Grid();
            this._rowpanel.Name = "_rowpanel";
            this._rowpanel.DataContext = this;
            this._rowpanel.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._rowpanel.SetBinding(Grid.VisibilityProperty, new Binding("Visibility"));
            this._rowpanel.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this._rowpanel.SetBinding(Grid.HeightProperty, new Binding("Style.Height"));
            this._rowpanel.VerticalAlignment = VerticalAlignment.Top;

            this.LoadXml(SourceXml);
        }
        #endregion

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            this.LoadXml(InputXml, this);
        }

        public override void LoadXml(XElement InputXml, ParentLayoutElement parent)
        {
            int colIndex = 0;

            foreach (XElement x in InputXml.Elements())
            {
                if (x.Name == "Column")
                {
                    TsColumn c = new TsColumn(x, colIndex, parent);

                    this._columns.Add(c);

                    ColumnDefinition coldef = new ColumnDefinition();
                    coldef.Width = GridLength.Auto;
                    this._rowpanel.ColumnDefinitions.Add(coldef);
                    Grid.SetColumn(c.Panel, colIndex);

                    this._rowpanel.Children.Add(c.Panel);
                    this._options.AddRange(c.Options);

                    colIndex++;
                }
                else if (x.Name == "Container")
                {
                    new UIContainer(this, x);
                }
            }
        }
    }
}
