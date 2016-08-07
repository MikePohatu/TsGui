using System;
using System.Windows;

namespace TsGui
{
    public interface IToggleControl
    {
        string VariableName { get; }
        string CurrentValue { get; }
        void AttachToggle(Toggle Toggle);
        event ToggleEvent ValueChange;
        void InitialiseToggle();
    }
}
