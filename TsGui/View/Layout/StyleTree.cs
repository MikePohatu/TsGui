
using Core.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TsGui.View.Layout
{
    /// <summary>
    /// StyleTree acts as a 'root' for the Label and Control styles and other items for GuiOptions. This
    /// extends the 'Style' type as it provides all the same options at the root of the LayoutElement. 
    /// Replaces the old 'Formatting' object type
    /// </summary>
    public class StyleTree: Style
    {   
        /// <summary>
        /// record the elements set by the user in the XML
        /// </summary>
        private Dictionary<string, bool> _setElements = new Dictionary<string, bool>();

        public Style LabelStyle { get; set; } = new Style();
        public Style ControlStyle { get; set; } = new Style();

        public bool LabelOnRight { get; set; }

        private string _id;
        public string ID
        {
            get { return this._id; }
            set { this._id = value; this.OnPropertyChanged(this, "ID"); }
        }

        private double _leftcellwidth;
        public double LeftCellWidth
        {
            get { return this._leftcellwidth; }
            set { this._leftcellwidth = value; this.OnPropertyChanged(this, "LeftCellWidth"); }
        }


        private double _rightcellwidth;
        public double RightCellWidth
        {
            get { return this._rightcellwidth; }
            set { this._rightcellwidth = value; this.OnPropertyChanged(this, "RightCellWidth"); }
        }


        public StyleTree() { }

        public StyleTree(XElement InputXml) { this.LoadXml(InputXml); }


        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml, false);

            //record the set elements
            foreach (XElement el in InputXml.Elements())
            {
                string name = el.Name.ToString();
                if (this._setElements.ContainsKey(name) == false)
                {
                    this._setElements.Add(name, true);
                }
            }

            this.ID = XmlHandler.GetStringFromXml(InputXml, "ID", this.ID);
            if (string.IsNullOrWhiteSpace(this.ID) == false && this.ID.Contains(" ")) { throw new KnownException("Spaces are not supported in ID attributes: " + this.ID, null); }

            //Load legacy options
            this.LeftCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "LabelWidth", this.LeftCellWidth);
            this.RightCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "ControlWidth", this.RightCellWidth);

            //load current options
            this.LeftCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "LeftCellWidth", this.LeftCellWidth);
            this.RightCellWidth = XmlHandler.GetDoubleFromXml(InputXml, "RightCellWidth", this.RightCellWidth);
            this.LabelOnRight = XmlHandler.GetBoolFromXml(InputXml, "LabelOnRight", this.LabelOnRight);

            XElement subx;
            subx = InputXml.Element("Label");
            if (subx != null)
            {
                this.LabelStyle.LoadXml(subx);
            }

            subx = InputXml.Element("Control");
            if (subx != null)
            {
                this.ControlStyle.LoadXml(subx);
            }

            string styleids = XmlHandler.GetStringFromXml(InputXml, "Import", null);
            if (string.IsNullOrWhiteSpace(styleids) == false)
            {
                foreach (string id in styleids.Trim().Split(' '))
                {
                    StyleTree s = StyleLibrary.Get(id);
                    this.Import(s);
                }
            }
        }

        public new StyleTree Clone()
        {
            StyleTree st = new StyleTree();
            CloneTo(this, st);
            st.ControlStyle = this.ControlStyle?.Clone();
            st.LabelStyle = this.LabelStyle?.Clone();
            st.LeftCellWidth = this.LeftCellWidth;
            st.RightCellWidth = this.RightCellWidth;
            return st;
        }

        public void Import(string id)
        {
            StyleTree s = StyleLibrary.Get(id);
            this.Import(s);
        }

        public void Import(StyleTree tree)
        {
            if (tree == null) { throw new KnownException("Cannot import from null Style. Check for correct ID", null); }
            
            if (tree._setElements.ContainsKey("Control")) { this.ControlStyle.Import(tree.ControlStyle); }
            if (tree._setElements.ContainsKey("Label")) { this.LabelStyle.Import(tree.LabelStyle); }


            if (tree._setElements.ContainsKey("RightCellWidth")) { this.RightCellWidth = tree.RightCellWidth; }
            if (tree._setElements.ContainsKey("LeftCellWidth")) { this.LeftCellWidth = tree.LeftCellWidth; }
            base.Import(tree);
        }
    }
}
