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

// TsImage.cs - Used for displaying images

using System.Xml.Linq;
using System.Windows;
using System.Threading.Tasks;

using TsGui.Images;
using TsGui.View.Layout;
using MessageCrap;
using System.Collections.Generic;

namespace TsGui.View.GuiOptions
{
    public class TsImage : GuiOptionBase, IGuiOption
    {
        public Image Image { get; set; }
        public override IEnumerable<Variable> Variables { get { return null; } }
        public override string CurrentValue { get { return this.Image.MultiImage.CurrentFilePath; } }

        //Constructor
        public TsImage(XElement InputXml, ParentLayoutElement Parent) : base(Parent)
        {
            this.Control = new TsImageUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);  
            
            if (this.Image != null) { this.Image.MultiImage.ImageScalingUpdate += this.OnImageScalingUpdate; }        
        }


        //Methods
        public new void LoadXml(XElement InputXml)
        {
            //load the xml for the base class stuff
            base.LoadXml(InputXml);

            XElement x;
            x = InputXml.Element("Image");
            if (x != null)
            { this.CreateImage(x); }
        }
        
        public void OnImageScalingUpdate(object o, RoutedEventArgs e)
        {
            this.NotifyViewUpdate();
        }

        private void CreateImage(XElement InputXml)
        {
            this.Image = new Image(InputXml);
            //this.VariableName = this.Image.File;
        }

        private void SetDefaults()
        {
            this.Style.RightCellWidth = this.Style.LeftCellWidth + this.Style.RightCellWidth;
            this.Style.LeftCellWidth = 0;            
            this.ControlStyle.Padding = new Thickness(0);
            this.ControlStyle.Margin = new Thickness(0);
            this.ControlStyle.VerticalAlignment = VerticalAlignment.Center;
            this.ControlStyle.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public override async Task UpdateLinkedValueAsync(Message message) { await Task.CompletedTask; }
    }
}
