using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace TsGui
{
    public class TsDropDownListItem: ComboBoxItem
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public TsDropDownListItem(string Value, string Text)
        {
            Label control = new Label();
            this.Value = Value;
            this.Text = Text;
            this.Content = control;
            
            control.Content = this.Text;
            control.Padding = new Thickness(0);
            control.Margin = new Thickness(0);
            control.SetBinding(IsEnabledProperty, new Binding("IsEnabled"));
        }
    }
}
