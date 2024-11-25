namespace SampleDirectXForms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public class NativeWindow
    {
        public IntPtr Handle { get; private set; }

        private const int CW_USEDEFAULT = -1;
        private const uint COLOR_BACKGROUND = 1;
        private const uint COLOR_WINDOW = 5;
        private const uint CS_DBLCLKS = 8;
        private const uint CS_HREDRAW = 2;
        private const uint CS_USEDEFAULT = 0x80000000;
        private const uint CS_VREDRAW = 1;
        private const uint IDC_ARROW = 32512;
        private const uint IDC_CROSS = 32515;
        private const uint WM_DESTROY = 2;
        private const uint WM_LBUTTONDBLCLK = 0x0203;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_PAINT = 0x0f;
        private const uint WS_CHILD = 0x40000000;
        private const uint WS_OVERLAPPEDWINDOW = 0xcf0000;
        private const uint WS_VISIBLE = 0x10000000;

        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [DllImport("user32.dll")]
        private static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("gdi32.dll")]
        static extern IntPtr GetStockObject(StockObjects fnObject);
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

        private readonly WndProc delegWndProc = myWndProc;

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            ushort classAtom,
            //string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        public bool Create()
        {
            var wc = new WNDCLASSEX();

            wc.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            wc.style = (int)(CS_HREDRAW | CS_VREDRAW);
            wc.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = Marshal.GetHINSTANCE(GetType().Module);
            wc.hIcon = LoadIcon(IntPtr.Zero, new IntPtr((int)SystemIcons.IDI_APPLICATION));
            wc.hIconSm = IntPtr.Zero;
            wc.hCursor = LoadCursor(IntPtr.Zero, (int)IDC_ARROW);
            wc.hbrBackground = GetStockObject(StockObjects.WHITE_BRUSH);
            wc.lpszMenuName = null;
            wc.lpszClassName = "Sample";

            var classAtom = RegisterClassEx(ref wc);

            if (classAtom == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            Handle = CreateWindowEx(
                0,
                classAtom,
                "Sample",
                WS_OVERLAPPEDWINDOW,
                0,
                0,
                800,
                600,
                IntPtr.Zero,
                IntPtr.Zero,
                wc.hInstance,
                IntPtr.Zero);

            if (Handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            ShowWindow(Handle, 1);
            UpdateWindow(Handle);

            return true;
        }

        public void Run()
        {
            MSG msg;

            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }


        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("user32.dll")]
        static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        private static IntPtr myWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_DESTROY:
                    PostQuitMessage(0);
                    return IntPtr.Zero;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        [DllImport("user32.dll")]
        private static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
        private static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern bool UpdateWindow(IntPtr hWnd);
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public int style;

            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }
    }
}