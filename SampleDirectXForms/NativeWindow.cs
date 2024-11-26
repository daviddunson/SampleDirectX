namespace SampleDirectXForms;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

public class NativeWindow
{
    private readonly NativeMethods.WndProc wndProc = WndProc;

    public IntPtr Handle { get; private set; }

    public bool Create()
    {
        var wc = new NativeMethods.WNDCLASSEX
        {
            cbSize = Marshal.SizeOf(typeof(NativeMethods.WNDCLASSEX)),
            style = (int)(NativeMethods.CS_HREDRAW | NativeMethods.CS_VREDRAW),
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(this.wndProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = Marshal.GetHINSTANCE(this.GetType().Module),
            hIcon = NativeMethods.LoadIcon(IntPtr.Zero, new IntPtr((int)NativeMethods.SystemIcons.IDI_APPLICATION)),
            hIconSm = IntPtr.Zero,
            hCursor = NativeMethods.LoadCursor(IntPtr.Zero, (int)NativeMethods.IDC_ARROW),
            hbrBackground = NativeMethods.GetStockObject(NativeMethods.StockObjects.WHITE_BRUSH),
            lpszMenuName = null,
            lpszClassName = "Sample",
        };

        var classAtom = NativeMethods.RegisterClassEx(ref wc);

        if (classAtom == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        this.Handle = NativeMethods.CreateWindowEx(
            0,
            classAtom,
            "Sample", NativeMethods.WS_OVERLAPPEDWINDOW,
            0,
            0,
            800,
            600,
            IntPtr.Zero,
            IntPtr.Zero,
            wc.hInstance,
            IntPtr.Zero);

        if (this.Handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        NativeMethods.ShowWindow(this.Handle, 1);
        NativeMethods.UpdateWindow(this.Handle);

        return true;
    }

    public void Run()
    {
        while (NativeMethods.GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
        {
            NativeMethods.TranslateMessage(ref msg);
            NativeMethods.DispatchMessage(ref msg);
        }
    }

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case NativeMethods.WM_DESTROY:
                NativeMethods.PostQuitMessage(0);
                return IntPtr.Zero;
        }

        return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
    }

}