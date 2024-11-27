namespace SampleDirectXForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

internal class D3DApp
{
    private static readonly Dictionary<nint, D3DApp> AppInstances = new();
    private readonly GameTimer mTimer = new();
    private readonly NativeMethods.WndProc wndProc = WndProc;

    private float frameCnt = 0;
    private uint m4xMsaaQuality;
    private bool mAppPaused;
    private int mClientHeight;
    private int mClientWidth;
    private bool mEnable4xMsaa;
    private nint mhAppInst;
    private string mMainWndCaption;
    private bool mMaximized;
    private bool mMinimized;
    private bool mResizing;
    private float timeElapsed = 0.0f;

    ////private ID3D11Device md3dDevice;
    ////private D3D_DRIVER_TYPE md3dDriverType;
    ////private ID3D11DeviceContext md3dImmediateContext;
    ////private ID3D11Texture2D mDepthStencilBuffer;
    ////private ID3D11DepthStencilView mDepthStencilView;
    ////private ID3D11RenderTargetView mRenderTargetView;
    ////private D3D11_VIEWPORT mScreenViewport;
    ////private IDXGISwapChain mSwapChain;

    public D3DApp(nint hInstance)
    {
    }

    ~D3DApp()
    {
    }

    public nint AppInst => this.AppInst;

    public float AspectRatio => (float)this.mClientWidth / this.mClientHeight;

    public nint MainWnd { get; private set; }

    public static ushort HIWORD(nint value)
    {
        return (ushort)((value >> 16) & 0xFFFF);
    }

    public static ushort LOWORD(nint value)
    {
        return (ushort)(value & 0xFFFF);
    }

    public static long MAKELRESULT(int low, int high)
    {
        return ((long)high << 16) | (uint)low;
    }

    public virtual bool Init()
    {
        return this.InitMainWindow();
    }

    public int Run()
    {
        this.mTimer.Reset();

        var msg = new NativeMethods.NativeMessage();

        while (msg.msg != NativeMethods.WM_QUIT)
        {
            if (NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, (uint)NativeMethods.PeekMessageParams.PM_REMOVE))
            {
                NativeMethods.TranslateMessage(ref msg);
                NativeMethods.DispatchMessage(ref msg);
            }
            else
            {
                this.mTimer.Tick();

                if (!this.mAppPaused)
                {
                    this.CalculateFrameStats();
                    this.UpdateScene(this.mTimer.DeltaTime);
                    this.DrawScene();

                    Thread.Sleep(0);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        return (int)msg.wParam;
    }

    protected virtual void DrawScene()
    {
    }

    protected virtual nint MsgProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case NativeMethods.WM_ACTIVATE:
            {
                if (LOWORD(wParam) == NativeMethods.WA_INACTIVE)
                {
                    this.mAppPaused = true;
                    this.mTimer.Stop();
                }
                else
                {
                    this.mAppPaused = false;
                    this.mTimer.Start();
                }

                return 0;
            }

            case NativeMethods.WM_ENTERSIZEMOVE:
            {
                this.mAppPaused = true;
                this.mResizing = true;
                this.mTimer.Stop();
                return 0;
            }

            case NativeMethods.WM_EXITSIZEMOVE:
            {
                this.mAppPaused = false;
                this.mResizing = false;
                this.mTimer.Start();
                this.OnResize();
                return 0;
            }

            case NativeMethods.WM_GETMINMAXINFO:
                var minMaxInfo = Marshal.PtrToStructure<NativeMethods.MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.X = 300;
                minMaxInfo.ptMinTrackSize.Y = 200;
                Marshal.StructureToPtr(minMaxInfo, lParam, false);
                return 0;

            case NativeMethods.WM_MENUCHAR:
                // Don't beep when we alt-enter.
                return new nint(MAKELRESULT(0, NativeMethods.MNC_CLOSE));

            case NativeMethods.WM_LBUTTONDOWN:
            case NativeMethods.WM_MBUTTONDOWN:
            case NativeMethods.WM_RBUTTONDOWN:
                this.OnMouseDown(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;

            case NativeMethods.WM_LBUTTONUP:
            case NativeMethods.WM_MBUTTONUP:
            case NativeMethods.WM_RBUTTONUP:
                this.OnMouseUp(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;
            case NativeMethods.WM_MOUSEMOVE:
                this.OnMouseMove(wParam, LOWORD(lParam), HIWORD(lParam));
                return 0;

            case NativeMethods.WM_DESTROY:
                NativeMethods.PostQuitMessage(0);
                return 0;
        }

        return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
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

    protected virtual void UpdateScene(float dt)
    {
    }

    private static IntPtr WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        var handle = NativeMethods.GetWindowLongPtr(hWnd, NativeMethods.GWLP_USERDATA);

        if (AppInstances.TryGetValue(handle, out var instance))
        {
            return instance.MsgProc(hWnd, msg, wParam, lParam);
        }

        return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private void CalculateFrameStats()
    {
        this.frameCnt++;

        var gameTime = this.mTimer.GameTime;

        if (gameTime - this.timeElapsed < 1.0f)
        {
            return;
        }

        var fps = (float)this.frameCnt;
        var mspf = 1000.0f / fps;

        var text = $"{this.mMainWndCaption} FPS: {fps:0} Frame Time: {mspf:0.000000} Game Time: {gameTime:0}";
        NativeMethods.SetWindowText(this.MainWnd, text);

        this.frameCnt = 0;
        this.timeElapsed += 1.0f;
    }

    private bool InitDirect3D()
    {
        return false;
    }

    private bool InitMainWindow()
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

        this.MainWnd = NativeMethods.CreateWindowEx(
            0,
            classAtom,
            "Sample C#",
            NativeMethods.WS_OVERLAPPEDWINDOW,
            NativeMethods.CW_USEDEFAULT,
            NativeMethods.CW_USEDEFAULT,
            NativeMethods.CW_USEDEFAULT,
            NativeMethods.CW_USEDEFAULT,
            IntPtr.Zero,
            IntPtr.Zero,
            wc.hInstance,
            IntPtr.Zero);

        if (this.MainWnd == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        AppInstances[this.MainWnd] = this;
        NativeMethods.SetWindowLongPtr(this.MainWnd, NativeMethods.GWLP_USERDATA, this.MainWnd);
        NativeMethods.ShowWindow(this.MainWnd, 1);
        NativeMethods.UpdateWindow(this.MainWnd);

        return true;
    }
}