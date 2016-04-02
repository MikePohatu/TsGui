//class to look after displaying tooltips and alerts in the UI.
//useful for displaying validity messages for freetext controls, and help
//text for hover over 

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace TsGui
{
    public static class WindowAlerts
    {
        /// <summary>
        /// Show the ToolTip bound to the control
        /// </summary>
        /// <param name="control"></param>
        public static void ShowToolTip(Control Control)
        {
            if (Control != null)
            {
                ToolTip tt = Control.ToolTip as ToolTip;
                if (tt != null)
                {
                    tt.StaysOpen = true;
                    tt.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Display the ToolTip assigned to the control with the specified text.
        /// Creates a new ToolTip object for the control if one does not exist. 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Message"></param>
        public static void ShowToolTip(Control Control, string Message)
        {
            ToolTip tt = SetToolTipText(Control, Message);
            tt.StaysOpen = true;
            tt.IsOpen = true;
        }

        /// <summary>
        /// Set the text of the ToolTip bound to a control
        /// </summary>
        /// <param name="Control"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static ToolTip SetToolTipText(Control Control, string Message)
        {
            ToolTip tt = Control.ToolTip as ToolTip;
            if (tt == null)
            {
                tt = new ToolTip();
                tt.Placement = PlacementMode.Right;
                tt.HorizontalOffset = 5;
                tt.PlacementTarget = Control;
                tt.Content = new TextBlock();
                Control.ToolTip = tt;
            }

            TextBlock tb = tt.Content as TextBlock;

            tb.Text = Message;
            tt.Content = tb;

            return tt;
        }
        /// <summary>
        /// Display a ToolTip, but don't assign it to the control
        /// Will create a new ToolTip if parameter is null. 
        /// Returns the ToolTip
        /// </summary>
        /// <param name="ToolTip"></param>
        /// <param name="TargetControl"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static ToolTip ShowUnboundToolTip(ToolTip ToolTip, Control TargetControl, string Message)
        {
            if (ToolTip == null) { ToolTip = new ToolTip(); }
            

            ToolTip.Placement = PlacementMode.Right;
            ToolTip.HorizontalOffset = 5;
            ToolTip.PlacementTarget = TargetControl;
            ToolTip.Content = new TextBlock();

            TextBlock tb = ToolTip.Content as TextBlock;

            tb.Text = Message;
            ToolTip.Content = tb;
            ToolTip.StaysOpen = true;
            ToolTip.IsOpen = true;

            return ToolTip;
        }


        /// <summary>
        /// Hide a controls ToolTip. If a ToolTip is not set, does nothing
        /// </summary>
        /// <param name="Control"></param>
        public static void HideToolTip(Control Control)
        {
            ToolTip tt = Control.ToolTip as ToolTip;
            if (tt != null)
            {
                tt.StaysOpen = false;
                tt.IsOpen = false;
            }
        }

        /// <summary>
        /// Hide a specific ToolTip
        /// </summary>
        /// <param name="ToolTip"></param>
        public static void HideToolTip (ToolTip ToolTip)
        {
            Debug.WriteLine("HideToolTip called");
            if (ToolTip != null)
            {
                Debug.WriteLine("ToolTip not null");
                ToolTip.StaysOpen = false;
                ToolTip.IsOpen = false;
            }
        }
    }
}
