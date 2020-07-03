using System;
using System.Runtime.InteropServices;

namespace MiniblinkNet.Windows
{
    internal class WinApi
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLongW", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong86(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong64(IntPtr hwnd, int nIndex);

        public static int GetWindowLong(IntPtr hwnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetWindowLong64(hwnd, nIndex);
            }
            return GetWindowLong86(hwnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong86(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLong64(IntPtr hwnd, int nIndex, int dwNewLong);

        public static IntPtr SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLong64(hwnd, nIndex, dwNewLong);
            }
            return SetWindowLong86(hwnd, nIndex, dwNewLong);
        }

        [DllImport("imm32.dll", EntryPoint = "ImmGetContext")]
        public static extern IntPtr ImmGetContext(IntPtr hwnd);

        [DllImport("imm32.dll", EntryPoint = "ImmSetCompositionWindow")]
        public static extern int ImmSetCompositionWindow(IntPtr himc, ref CompositionForm lpCompositionForm);

        [DllImport("imm32.dll", EntryPoint = "ImmReleaseContext")]
        public static extern int ImmReleaseContext(IntPtr hwnd, IntPtr himc);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow")]
        public static extern int UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref WinPoint pptDst, ref WinSize psize,
            IntPtr hdcSrc, ref WinPoint pptSrc, int crKey, ref BlendFunction pblend, int dwFlags);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern int DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [DllImport("user32.dll", EntryPoint = "SetCapture")]
        public static extern int SetCapture(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern int ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        public static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("user32.dll ", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
