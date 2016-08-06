using System;
using System.Windows;

namespace TsGui
{
    public delegate void ToggleEventHandler(IToggleControl c, RoutedEventArgs e);

    public interface IToggleControl
    {
        string VariableName { get; }
        string CurrentValue { get; }
        void AttachToggle(Toggle Toggle);
        event ToggleEventHandler ValueChange;
        void InitialiseToggle();
    }
}
