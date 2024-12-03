namespace SampleDirectXForms;

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal class NativeMethods
{
    public const uint COLOR_BACKGROUND = 1;
    public const uint COLOR_WINDOW = 5;
    public const uint CS_DBLCLKS = 8;
    public const uint CS_HREDRAW = 2;
    public const uint CS_USEDEFAULT = 0x80000000;
    public const uint CS_VREDRAW = 1;
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);
    public const int GWLP_USERDATA = -21;
    public const uint IDC_ARROW = 32512;
    public const uint IDC_CROSS = 32515;
    public const int MNC_CLOSE = 0x0003;
    public const int WA_ACTIVE = 1;
    public const int WA_CLICKACTIVE = 2;
    public const int WA_INACTIVE = 0;
    public const uint WM_ACTIVATE = 0x6;
    public const uint WM_DESTROY = 2;
    public const uint WM_ENTERSIZEMOVE = 0x231;
    public const uint WM_EXITSIZEMOVE = 0x232;
    public const uint WM_GETMINMAXINFO = 0x24;
    public const uint WM_LBUTTONDBLCLK = 0x0203;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_MBUTTONDOWN = 0x0207;
    public const uint WM_MBUTTONUP = 0x0208;
    public const uint WM_MENUCHAR = 0x120;
    public const uint WM_MOUSEMOVE = 0x0200;
    public const uint WM_PAINT = 0x0f;
    public const uint WM_QUIT = 0x12;
    public const uint WM_RBUTTONDOWN = 0x0204;
    public const uint WM_RBUTTONUP = 0x0205;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_OVERLAPPEDWINDOW = 0xcf0000;
    public const uint WS_VISIBLE = 0x10000000;

    public delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [Flags]
    public enum PeekMessageParams : uint
    {
        PM_NOREMOVE = 0x0000,
        PM_REMOVE = 0x0001,
    }

    public enum StockObjects
    {
        WHITE_BRUSH = 0,
        LTGRAY_BRUSH = 1,
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = NULL_BRUSH,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,
        DEFAULT_GUI_FONT = 17,
        DC_BRUSH = 18,
        DC_PEN = 19,
    }

    public enum SystemIcons
    {
        IDI_APPLICATION = 32512,
        IDI_HAND = 32513,
        IDI_QUESTION = 32514,
        IDI_EXCLAMATION = 32515,
        IDI_ASTERISK = 32516,
        IDI_WINLOGO = 32517,
        IDI_WARNING = IDI_EXCLAMATION,
        IDI_ERROR = IDI_HAND,
        IDI_INFORMATION = IDI_ASTERISK,
    }

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
    public static extern nint CreateWindowEx(
        int dwExStyle,
        ushort lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);

    [DllImport("user32.dll")]
    public static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint DispatchMessage([In] ref NativeMessage lpmsg);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();

    [DllImport("user32.dll")]
    public static extern sbyte GetMessage(out NativeMessage lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("gdi32.dll")]
    public static extern nint GetStockObject(StockObjects fnObject);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

    public static ushort HIWORD(nint value)
    {
        return (ushort)((value >> 16) & 0xFFFF);
    }

    [DllImport("user32.dll")]
    public static extern nint LoadCursor(nint hInstance, int lpCursorName);

    [DllImport("user32.dll")]
    public static extern nint LoadIcon(nint hInstance, nint lpIconName);

    public static ushort LOWORD(nint value)
    {
        return (ushort)(value & 0xFFFF);
    }

    public static long MAKELRESULT(int low, int high)
    {
        return ((long)high << 16) | (uint)low;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessage(
        out NativeMessage lpMsg,
        nint hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint wRemoveMsg);

    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
    public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

    // P/Invoke for SetWindowLongPtr
    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetWindowText(nint hwnd, string lpString);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage([In] ref NativeMessage lpMsg);

    [DllImport("user32.dll")]
    public static extern bool UpdateWindow(nint hWnd);

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage
    {
        public nint handle;
        public uint msg;
        public nint wParam;
        public nint lParam;
        public uint time;
        public Point p;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT(int x, int y)
    {
        public int X = x;
        public int Y = y;

        public static implicit operator Point(POINT p)
        {
            return new Point(p.X, p.Y);
        }

        public static implicit operator POINT(Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;

        [MarshalAs(UnmanagedType.U4)]
        public int style;

        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;
    }
}