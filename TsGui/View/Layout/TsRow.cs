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

// TsRow.cs - class for rows in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsRow : BaseLayoutElement
    {
        private Grid _rowpanel;
        private List<TsColumn> _columns = new List<TsColumn>();
        private TsPage _parent;
        private List<IGuiOption_2> _options = new List<IGuiOption_2>();

        //properties
        #region
        public List<IGuiOption_2> Options { get { return this._options; } }
        public TsPage Parent { get { return this._parent; } }
        public int Index { get; set; }
        public Panel Panel { get { return this._rowpanel; } }
        #endregion

        //constructor
        #region
        public TsRow (XElement SourceXml,int PageIndex, TsPage Parent, MainController MainController): base (Parent,MainController)
        {

            this._controller = MainController;
            this._parent = Parent;
            this.Index = PageIndex;
            this._rowpanel = new Grid();
            this._rowpanel.Name = "_rowpanel";
            this._rowpanel.DataContext = this;
            this._rowpanel.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this._rowpanel.SetBinding(Grid.HeightProperty, new Binding("GridFormatting.Height"));
            this._rowpanel.SetBinding(Grid.WidthProperty, new Binding("GridFormatting.Width"));
            this._rowpanel.VerticalAlignment = VerticalAlignment.Top;

            this.LoadXml(SourceXml);
        }
        #endregion

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;
            int colIndex = 0;

            this.GridFormatting.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.GridFormatting.Height);

            xlist = InputXml.Elements("Column");
            if (xlist != null)
            {
                foreach (XElement xColumn in xlist)
                {
                    TsColumn c = new TsColumn(xColumn, colIndex, this, this._controller);

                    this._columns.Add(c);

                    ColumnDefinition coldef = new ColumnDefinition();
                    coldef.Width = GridLength.Auto;
                    this._rowpanel.ColumnDefinitions.Add(coldef);
                    Grid.SetColumn(c.Panel, colIndex);

                    this._rowpanel.Children.Add(c.Panel);
                    this._options.AddRange(c.Options);
                    this.GroupableHide += c.OnParentHide;
                    this.GroupableEnable += c.OnParentEnable;

                    colIndex++;
                }
            }
        }
    }
}
