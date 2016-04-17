using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TsGui
{
    public class ComboBoxEx : ComboBox
    {
        private int _selected;
        private bool _isloaded = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _selected = SelectedIndex;
            SelectedIndex = -1;

            Loaded += ComboBoxEx_Loaded;
        }

        void ComboBoxEx_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isloaded)
            {
                var popup = GetTemplateChild("PART_Popup") as Popup;
                var content = popup.Child as FrameworkElement;
                content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                MinWidth = content.DesiredSize.Width + ActualWidth;
                SelectedIndex = _selected;
                this._isloaded = true;
            }
        }

    }
}
