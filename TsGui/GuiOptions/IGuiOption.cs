using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace TsGui
{
    public interface IGuiOption: ITsGuiElement
    {
        //event PropertyChangedEventHandler PropertyChanged;

        TsVariable Variable { get; }
        Label Label { get; }
        Control Control { get; }
        Thickness Padding { get; }
        Thickness Margin { get; }
        Thickness LabelMargin { get; }
        Thickness LabelPadding { get; }
        int Height { get; }
        //DataGridLength 
        //string HelpText { get; set; }
    }
}
