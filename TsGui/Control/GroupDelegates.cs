using System.Windows;


namespace TsGui
{
    public delegate void ToggleEvent(IToggleControl c, RoutedEventArgs e);

    /// <summary>
    /// Event for notifying sub controls that the parents visibility has changed.
    /// 0 = Enabled
    /// 1 = Disabled
    /// 2 = Hidden
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="Visibility"></param>
    public delegate void ParentToggleEvent(IGroupParent Parent, int Visibility);
}
