using System.Runtime.InteropServices;

namespace TsGui.Connectors.System
{
    public static class Kernel32Methods
    {
        [DllImport("Kernel32.dll")]
        internal static extern bool AttachConsole(int processId);

        [DllImport("Kernel32.dll")]
        internal static extern int FreeConsole();
    }
}
