using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ClockWidget
{
    /// <summary>
    /// Class for window handle and control.
    /// </summary>
    public static class WindowUtility
    {
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = -20;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int WM_SETFOCUS = 0x0007;

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// Get the window extended style.
        /// </summary>
        /// <param name="window">The Window</param>
        /// <returns>Extended style</returns>
        public static int GetExtendedStyle(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            return GetWindowLong(hWnd, GWL_EXSTYLE);
        }

        /// <summary>
        /// Set the window to transparent and ignore mouse events.
        /// </summary>
        /// <param name="window">The Window</param>
        /// <param name="extendedStyle">Extended style</param>
        public static void SetWindowTransparent(Window window, int extendedStyle)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Set the window back to normal and receive mouse events.
        /// </summary>
        /// <param name="window">The Window</param>
        /// <param name="extendedStyle">Extended style</param>
        public static void SetWindowNormal(Window window, int extendedStyle)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            SetWindowLong(hWnd, GWL_EXSTYLE, extendedStyle);
        }

        /// <summary>
        /// Send the window to the back.
        /// </summary>
        /// <param name="window">The Window</param>
        public static void SendWindowToBack(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// Send the window to the front.
        /// </summary>
        /// <param name="window">The Window</param>
        public static void SendWindowToFront(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }
    }
}
