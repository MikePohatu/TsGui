using System.Windows.Controls;
using System.ComponentModel;

namespace TsGui
{
    public interface IGuiOption: ITsGuiElement
    {
        //event PropertyChangedEventHandler PropertyChanged;

        TsVariable Variable { get; }
        Label Label { get; }
        Control Control { get; }
        int Height { get; }
        //string HelpText { get; set; }
    }
}
