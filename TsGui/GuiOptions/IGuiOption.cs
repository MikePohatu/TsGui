﻿using System.Windows.Controls;
using System.Windows;

namespace TsGui
{
    public interface IGuiOption: ITsGuiElement, IGroupable
    {
        TsVariable Variable { get; }
        string VariableName { get; }
        string InactiveValue { get; }
        Label Label { get; }
        Control Control { get; }
        Thickness Padding { get; }
        Thickness Margin { get; }
        Thickness LabelMargin { get; }
        Thickness LabelPadding { get; }
        int Height { get; }

        void OnParentChanged(IGroupParent p, bool IsEnabled, bool IsHidden);
    }
}
