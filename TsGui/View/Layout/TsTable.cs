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
using System.Windows;

using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class TsTable : BaseLayoutElement
    {
        private List<TsRow> _rows = new List<TsRow>();
        private List<IGuiOption> _options = new List<IGuiOption>();
        private List<IValidationGuiOption> _validationoptions = new List<IValidationGuiOption>();
        private Grid _grid;

        //Properties
        #region
        public List<IGuiOption> Options { get { return this._options; } }
        public Grid Grid { get { return this._grid; } }
        #endregion

        //Constructors
        public TsTable(XElement SourceXml, BaseLayoutElement Parent, MainController MainController, Grid ExistingGrid) : base(MainController)
        {
            this.Init(SourceXml, Parent, MainController, ExistingGrid);
        }

        public TsTable(XElement SourceXml, BaseLayoutElement Parent, MainController MainController) : base(MainController)
        {
            Grid g = new Grid();
            this.Init(SourceXml, Parent, MainController, g);
        }

        //methods
        private void Init(XElement SourceXml, BaseLayoutElement Parent, MainController MainController, Grid MainGrid)
        {
            this._grid = MainGrid;
            this.Parent = Parent;
            this._controller = MainController;
            this.ShowGridLines = MainController.ShowGridLines;

            this.LoadXml(SourceXml);
            this.PopulateOptions();
            this.Build();
        }


        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;
            XElement x;
            int index;

            this.IsEnabled = XmlHandler.GetBoolFromXElement(InputXml, "Enabled", this.IsEnabled);
            this.IsHidden = XmlHandler.GetBoolFromXElement(InputXml, "Hidden", this.IsHidden);
            this.ShowGridLines = XmlHandler.GetBoolFromXElement(InputXml, "ShowGridLines", this.Parent.ShowGridLines);

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
            if (0 == i)
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
            TsRow r = new TsRow(InputXml, Index, this, this._controller);

            this._rows.Add(r);
        }

        //build the gui controls.
        public void Build()
        {
            int index = 0;

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
