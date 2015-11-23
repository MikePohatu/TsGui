using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace core
{
    public interface IGuiOption
    {
        TsVariable Variable { get; }
        string Label { get; }
        Control Control { get; }
        bool LoadXml(XElement Xml);
    }
}
