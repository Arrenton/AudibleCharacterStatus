using System;
using System.Runtime.InteropServices;

namespace AudibleCharacterStatus
{
    public static class ProcessUtils
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        public static uint GetForegroundProcessId()
        {
            IntPtr hWnd = GetForegroundWindow(); // Get foreground window handle
            GetWindowThreadProcessId(hWnd, out var processId);
            return processId;
        }
    }
}
