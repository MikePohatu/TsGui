using System;
using System.Xml.Linq;
using System.Windows.Controls;

namespace core
{
    public interface IGuiOption: ITsGuiElement
    {
        TsVariable Variable { get; }
        string Label { get; }
        Control Control { get; }
    }
}
