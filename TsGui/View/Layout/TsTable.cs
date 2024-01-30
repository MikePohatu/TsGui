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

// TsTable.cs - builds the grid structure and populates with GuiOtions

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

using TsGui.View.GuiOptions;
using System;

namespace TsGui.View.Layout
{
    public class TsTable
    {
        private List<TsRow> _rows = new List<TsRow>();
        private ParentLayoutElement _parent;
        private int _rowcount = 0;

        //Properties
        #region
        public List<IGuiOption> Options { get; } = new List<IGuiOption>();
        public List<IValidationGuiOption> ValidationOptions { get; } = new List<IValidationGuiOption>();
        public Grid Grid { get; private set; }
        #endregion

        //Constructors
        public TsTable(XElement SourceXml, ParentLayoutElement Parent)
        {
            this._parent = Parent; 
            this.Grid = new Grid();
            this.Grid.Name = "_tablegrid";
            this.Grid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this.Grid.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this.Grid.VerticalAlignment = VerticalAlignment.Top;
            this.Grid.HorizontalAlignment = HorizontalAlignment.Left;

            this.LoadXml(SourceXml);
            Director.Instance.ConfigLoadFinished += this.OnConfigLoadFinished;
        }

        //Methods
        public void LoadXml(XElement InputXml)
        {
            //base.LoadXml(InputXml);
            IEnumerable<XElement> xlist;
            XElement x;

            //now read in the options and add to a dictionary for later use
            xlist = InputXml.Elements("Row");
            if (xlist != null)
            {
                foreach (XElement xrow in xlist)
                {
                    this.CreateRow(xrow, _rowcount);
                    this._rowcount++;
                }
            }

            //legacy support i.e. no row in config.xml. create a new row and add the columns 
            //to it
            if (this._rowcount == 0)
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

        //deregister this because a new table will be built
        private void OnReload(object sender, EventArgs e)
        {
            Director.Instance.ConfigLoadFinished -= this.OnConfigLoadFinished;
            Director.Instance.Reloaded -= this.OnReload;
        }

        private void OnConfigLoadFinished(object sender, EventArgs e)
        {
            Director.Instance.Reloaded += this.OnReload;
            this.Build();
        }

        //build the gui controls.
        public void Build()
        {
            this.PopulateOptions();

            int index = 0;

            foreach (TsRow row in this._rows)
            {
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;

                this.Grid.RowDefinitions.Add(rowdef);

                Grid.SetRow(row.Panel, index);
                this.Grid.Children.Add(row.Panel);
                index++;
            }
        }

        //get all the options from the sub columns. this is parsed up the chain to generate the final
        //list of ts variables to set at the end. 
        private void PopulateOptions()
        {
            foreach (TsRow row in this._rows)
            {
                this.Options.AddRange(row.Options);
            }

            foreach (IGuiOption option in this.Options)
            {
                if (option is IValidationGuiOption) { this.ValidationOptions.Add((IValidationGuiOption)option); }
            }
        }
    }
}
