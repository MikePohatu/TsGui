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

// TsTable.cs - builds the grid structure and populates with GuiOtions

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsTable
    {
        private List<TsRow> _rows = new List<TsRow>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IValidationGuiOption> _validationoptions = new List<IValidationGuiOption>();
        private Grid _grid;
        private BaseLayoutElement _parent;

        //Properties
        #region
        public List<IGuiOption> Options { get { return this._options; } }
        public List<IValidationGuiOption> ValidationOptions { get { return this._validationoptions; } }
        public Grid Grid { get { return this._grid; } }
        #endregion

        //Constructors
        public TsTable(XElement SourceXml, BaseLayoutElement Parent, Grid ExistingGrid)
        {
            this.Init(SourceXml, Parent, ExistingGrid);
        }

        public TsTable(XElement SourceXml, BaseLayoutElement Parent)
        {
            
            Grid g = new Grid();
            g.Name = "_tablegrid";
            this.Init(SourceXml, Parent, g);
        }

        //methods
        private void Init(XElement SourceXml, BaseLayoutElement Parent, Grid Grid)
        {
            this._parent = Parent;
            this._grid = Grid;
            this.LoadXml(SourceXml);
            this.PopulateOptions();
            this.Build();
        }


        //Methods
        public void LoadXml(XElement InputXml)
        {
            //base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;
            XElement x;
            int index;

            //now read in the options and add to a dictionary for later use
            int i = 0;
            xlist = InputXml.Elements("Row");
            if (xlist != null)
            {
                index = 0;
                foreach (XElement xrow in xlist)
                {
                    this.CreateRow(xrow, index);
                    index++;
                    i++;
                }
            }

            //legacy support i.e. no row in config.xml. create a new row and add the columns 
            //to it
            if (i == 0)
            {

                xlist = InputXml.Elements("Column");
                x = new XElement("Row");

                foreach (XElement xColumn in xlist)
                {
                    x.Add(xColumn);
                }
                if (x.Elements() != null) { this.CreateRow(x, 0); }
            }

        }

        private void CreateRow(XElement InputXml, int Index)
        {
            TsRow r = new TsRow(InputXml, Index, this._parent);
            this._rows.Add(r);
        }

        //build the gui controls.
        public void Build()
        {
            int index = 0;

            this._grid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this._grid.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._grid.VerticalAlignment = VerticalAlignment.Top;
            this._grid.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (TsRow row in this._rows)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;

                this._grid.RowDefinitions.Add(rowdef);

                Grid.SetRow(row.Panel, index);
                this._grid.Children.Add(row.Panel);
                index++;
            }
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (TsRow row in this._rows)
            {
                this._options.AddRange(row.Options);
            }

            foreach (IGuiOption option in this._options)
            {
                if (option is IValidationGuiOption) { this._validationoptions.Add((IValidationGuiOption)option); }
            }
        }
    }
}
