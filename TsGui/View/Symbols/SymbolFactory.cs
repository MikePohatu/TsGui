using System.Windows.Controls;
using System;
using TsGui.Diagnostics.Logging;

namespace TsGui.View.Symbols
{
    public static class SymbolFactory
    {
        public static UserControl GetSymbol(string type)
        {
            if (type == null) { throw new ArgumentException("Missing Type attribute on Symbol"); }

            LoggerFacade.Info("Creating symbol, type: " + type);

            if (type == "OuUI")
            {
                return new TsOuUI();
            }
            else if (type == "Cross")
            {
                return new TsCrossUI();
            }
            else if (type == "Tick")
            {
                return new TsTickUI();
            }
            else if (type == "Warn")
            {
                return new TsWarnUI();
            }
            else if (type == "TrafficLight")
            {
                return new TsTrafficLightUI();
            }
            else { return null; }
        }

        public static UserControl Copy(UserControl control)
        {
            if (control == null) { return null; }

            string type = control.GetType().ToString();
            LoggerFacade.Info("Creating symbol, type: " + type);

            if (type == "TsGui.View.Symbols.TsOuUI")
            {
                return new TsOuUI();
            }
            else if (type == "TsGui.View.Symbols.TsCrossUI")
            {
                return new TsCrossUI();
            }
            else if (type == "TsGui.View.Symbols.TsTickUI")
            {
                return new TsTickUI();
            }
            else if (type == "TsGui.View.Symbols.TsWarnUI")
            {
                return new TsWarnUI();
            }
            else if (type == "TsGui.View.Symbols.TsTrafficLightUI")
            {
                return new TsTrafficLightUI();
            }
            else { return null; }
        }
    }
}
