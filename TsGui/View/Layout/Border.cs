using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using TsGui.View.GuiOptions;

namespace TsGui.View.Layout
{
    public class Border : ViewModelBase
    {
        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get { return this._brush; }
            set { this._brush = value; this.OnPropertyChanged(this, "Brush"); }
        }

        private Thickness _thickness = new Thickness(0);
        public Thickness Thickness
        {
            get { return this._thickness; }
            set
            {
                this._thickness = value;
                this.OnPropertyChanged(this, "Thickness");
            }
        }

        public void LoadXml(XElement InputXml)
        {
            if (InputXml != null)
            {
                this.Brush = XmlHandler.GetSolidColorBrushFromXml(InputXml, "Color", this._brush);
                this.Thickness = XmlHandler.GetThicknessFromXml(InputXml, "Thickness", this._thickness);
            }
        }
    }
}
