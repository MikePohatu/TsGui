//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

//  TsWindowAlert.cs - class to look after displaying tooltips and alerts in the UI.
//  useful for displaying validity messages for freetext controls, and help
//  text for hover over 

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TsGui
{
    public static class TsWindowAlerts
    {
        /// <summary>
        /// Set the text of the ToolTip bound to a control. Will create a new ToolTip 
        /// and TextBlock if not already created. Defaults to mouse placement mode
        /// </summary>
        /// <param name="Control"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static ToolTip SetToolTipText(Control Control, string Message)
        {
            if (string.IsNullOrEmpty(Message)) { return null; }
            if (Control == null ) { return null; }

            ToolTip tt = Control.ToolTip as ToolTip;
            if (tt == null)
            {
                tt = CreateToolTip(Message);
                Control.ToolTip = tt;
            }

            return tt;
        }

        /// <summary>
        /// Show the ToolTip bound to the control, if it exists
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
        /// Display the ToolTip assigned to the control and keep it open,with the specified text.
        /// Creates a new ToolTip object for the control if one does not exist. 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Message"></param>
        public static ToolTip ShowToolTip(Control Control, string Message)
        {
            if (string.IsNullOrEmpty(Message)) { return null; }
            ToolTip tt = SetToolTipText(Control, Message);
            tt.PlacementTarget = Control;

            tt.StaysOpen = true;
            tt.IsOpen = true;
            return tt;
        }

        /// <summary>
        /// Display a ToolTip and keep it open, but don't assign it to the control
        /// Will create a new ToolTip if parameter is null. 
        /// Returns the ToolTip
        /// </summary>
        /// <param name="ToolTip"></param>
        /// <param name="TargetControl"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static ToolTip ShowUnboundToolTip(ToolTip ToolTip, Control TargetControl, string Message)
        {
            ToolTip tt = ToolTip;
            if (tt == null) { tt = CreateToolTip(Message); }        
            else { UpdateToolTipMessage(tt, Message); }
            tt.PlacementTarget = TargetControl;

            tt.StaysOpen = true;
            tt.IsOpen = true;

            return tt;
        }


        /// <summary>
        /// Hide a control's ToolTip. If a ToolTip is not set, does nothing
        /// </summary>
        /// <param name="Control"></param>
        public static void HideToolTip(Control Control)
        {
            if (Control != null)
            {
                ToolTip tt = Control.ToolTip as ToolTip;
                if (tt != null)
                {
                    tt.StaysOpen = false;
                    tt.IsOpen = false;
                }
            }
        }

        /// <summary>
        /// Hide a specific ToolTip
        /// </summary>
        /// <param name="ToolTip"></param>
        public static void HideToolTip (ToolTip ToolTip)
        {
            //Debug.WriteLine("HideToolTip called");
            if (ToolTip != null)
            {
                ToolTip.StaysOpen = false;
                ToolTip.IsOpen = false;
            }
        }

        //create a new tooltip and set it up
        private static ToolTip CreateToolTip(string Message)
        {
            ToolTip tt = new ToolTip();

            tt.Placement = PlacementMode.Mouse;
            tt.HorizontalOffset = 5;
            UpdateToolTipMessage(tt, Message);

            return tt;
        }

        //update the text of the tooltip
        private static void UpdateToolTipMessage(ToolTip ToolTip, string Message)
        {
            if (ToolTip == null) { return; }
            if (ToolTip.Content == null) { ToolTip.Content = new TextBlock(); }

            TextBlock tb = ToolTip.Content as TextBlock;

            tb.Text = Message;
        }
    }
}
