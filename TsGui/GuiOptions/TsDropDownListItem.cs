using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace TsGui
{
    public class TsDropDownListItem: ComboBoxItem
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public TsDropDownListItem(string Key, string Value)
        {
            Label control = new Label();
            this.Key = Key;
            this.Value = Value;
            
            control.Content = this.Value;
            control.Padding = new Thickness(0);
            control.Margin = new Thickness(0);
            //control.SetBinding(Label.ContentProperty, new Binding("Value"));
            control.SetBinding(IsEnabledProperty, new Binding("IsEnabled"));
            //control.DataContext = this;
            this.Content = control;
        }
    }
}
