using System;
using System.Runtime.InteropServices;

namespace MiniblinkNet
{
    internal class WinApi
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLongW", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong86(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong64(IntPtr hwnd, int nIndex);

        internal static int GetWindowLong(IntPtr hwnd, int nIndex)
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

        internal static IntPtr SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLong64(hwnd, nIndex, dwNewLong);
            }
            return SetWindowLong86(hwnd, nIndex, dwNewLong);
        }

        [DllImport("imm32.dll", EntryPoint = "ImmGetContext")]
        internal static extern IntPtr ImmGetContext(IntPtr hwnd);

        [DllImport("imm32.dll", EntryPoint = "ImmSetCompositionWindow")]
        internal static extern int ImmSetCompositionWindow(IntPtr himc, ref CompositionForm lpCompositionForm);

        [DllImport("imm32.dll", EntryPoint = "ImmReleaseContext")]
        internal static extern int ImmReleaseContext(IntPtr hwnd, IntPtr himc);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        internal static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow")]
        internal static extern int UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, ref WinPoint pptDst, ref WinSize psize,
            IntPtr hdcSrc, ref WinPoint pptSrc, int crKey, ref BlendFunction pblend, int dwFlags);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        internal static extern int DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern int DeleteDC(IntPtr hdc);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        [DllImport("user32.dll", EntryPoint = "SetCapture")]
        internal static extern int SetCapture(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        internal static extern int ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SetFocus")]
        internal static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("user32.dll ", EntryPoint = "SendMessage")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "BeginPaint")]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, ref WinPaint lpPaint);

        [DllImport("user32.dll", EntryPoint = "EndPaint")]
        internal static extern int EndPaint(IntPtr hwnd, ref WinPaint lpPaint);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        internal static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc,
            int ySrc, int dwRop);
    }
}
