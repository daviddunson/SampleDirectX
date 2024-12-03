namespace SampleDirectXForms;

using System.Collections.Generic;
using System.ComponentModel;
using static System.Runtime.InteropServices.Marshal;
using static NativeMethods;

internal class D3DWindow
{
    private static readonly Dictionary<nint, D3DWindow> Instances = new();
    private readonly nint instanceHandle;
    private readonly WndProc wndProc = WndProc;
    ////private bool mMaximized;
    ////private bool mMinimized;
    ////private bool mResizing;

    protected D3DWindow(nint instanceHandle)
    {
        this.instanceHandle = instanceHandle;
    }

    public float AspectRatio => (float)this.ClientWidth / this.ClientHeight;

    public string Caption { get; set; } = "Sample C#";

    public int ClientHeight { get; private set; }

    public int ClientWidth { get; private set; }

    public nint Handle { get; private set; }

    public virtual void Init()
    {
        var wc = new WNDCLASSEX
        {
            cbSize = SizeOf(typeof(WNDCLASSEX)),
            style = (int)(CS_HREDRAW | CS_VREDRAW),
            lpfnWndProc = GetFunctionPointerForDelegate(this.wndProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = this.instanceHandle,
            hIcon = LoadIcon(nint.Zero, new nint((int)SystemIcons.IDI_APPLICATION)),
            hIconSm = nint.Zero,
            hCursor = LoadCursor(nint.Zero, (int)IDC_ARROW),
            hbrBackground = GetStockObject(StockObjects.WHITE_BRUSH),
            lpszMenuName = null,
            lpszClassName = "Sample",
        };

        var classAtom = RegisterClassEx(ref wc);

        if (classAtom == 0)
        {
            throw new Win32Exception(GetLastWin32Error());
        }

        this.Handle = CreateWindowEx(
            0,
            classAtom,
            this.Caption,
            WS_OVERLAPPEDWINDOW,
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            nint.Zero,
            nint.Zero,
            wc.hInstance,
            nint.Zero);

        if (this.Handle == nint.Zero)
        {
            throw new Win32Exception(GetLastWin32Error());
        }

        Instances[this.Handle] = this;

        ShowWindow(this.Handle, 1);
        UpdateWindow(this.Handle);
    }

    protected virtual nint MsgProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case WM_ACTIVATE:
            {
                if (LOWORD(wParam) == WA_INACTIVE)
                {
                    this.OnDeactivated();
                }
                else
                {
                    this.OnActivated();
                }

                return 0;
            }

            case WM_ENTERSIZEMOVE:
            {
                ////this.mResizing = true;
                this.OnResizing();
                return 0;
            }

            case WM_EXITSIZEMOVE:
            {
                ////this.mResizing = false;
                this.OnResize();
                return 0;
            }

            case WM_SIZE:
            {
                this.ClientWidth = (int)(lParam & 0xFFFF);
                this.ClientHeight = (int)((lParam >> 16) & 0xFFFF);
                break;
            }

            case WM_GETMINMAXINFO:
                var minMaxInfo = PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.X = 300;
                minMaxInfo.ptMinTrackSize.Y = 200;
                StructureToPtr(minMaxInfo, lParam, false);
                return 0;

            case WM_MENUCHAR:
                // Don't beep when we alt-enter.
                return new nint(MAKELRESULT(0, MNC_CLOSE));

            case WM_LBUTTONDOWN:
            case WM_MBUTTONDOWN:
            case WM_RBUTTONDOWN:
                this.OnMouseDown(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;

            case WM_LBUTTONUP:
            case WM_MBUTTONUP:
            case WM_RBUTTONUP:
                this.OnMouseUp(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;

            case WM_MOUSEMOVE:
                this.OnMouseMove(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;

            case WM_DESTROY:
                PostQuitMessage(0);
                return 0;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    protected virtual void OnActivated()
    {
    }

    protected virtual void OnDeactivated()
    {
    }

    protected virtual void OnMouseDown(nint btnState, int x, int y)
    {
    }

    protected virtual void OnMouseMove(nint btnState, int x, int y)
    {
    }

    protected virtual void OnMouseUp(nint btnState, int x, int y)
    {
    }

    protected virtual void OnResize()
    {
    }

    protected virtual void OnResizing()
    {
    }

    private static nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        return Instances.TryGetValue(hWnd, out var instance) ?
            instance.MsgProc(hWnd, msg, wParam, lParam) :
            DefWindowProc(hWnd, msg, wParam, lParam);
    }
}