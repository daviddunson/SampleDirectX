#include "D3DApp.h"

D3DApp::D3DApp(HINSTANCE hInstance) :
    mhAppInst(hInstance),
    mhMainWnd(nullptr),
    mAppPaused(false),
    mMinimized(false),
    mMaximized(false),
    mResizing(false),
    m4xMsaaQuality(0),
    md3dDevice(nullptr),
    md3dImmediateContext(nullptr),
    mSwapChain(nullptr),
    mDepthStencilBuffer(nullptr),
    mRenderTargetView(nullptr),
    mDepthStencilView(nullptr),
    mScreenViewport(),
    md3dDriverType(),
    mClientWidth(0),
    mClientHeight(0),
    mEnable4xMsaa(false)
{
}

D3DApp::~D3DApp()
{
}

HINSTANCE D3DApp::AppInst() const
{
    return mhAppInst;
}

HWND D3DApp::MainWnd() const
{
    return mhMainWnd;
}

float D3DApp::AspectRatio() const
{
    return static_cast<float>(mClientWidth) / mClientHeight;
}

int D3DApp::Run()
{
    mTimer.Reset();

    MSG msg = { nullptr };

    while (msg.message != WM_QUIT)
    {
        if (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
        else
        {
            mTimer.Tick();

            if (!mAppPaused)
            {
                CalculateFrameStats();
                UpdateScene(mTimer.DeltaTime());
                DrawScene();

                Sleep(1);
            }
            else
            {
                Sleep(100);
            }
        }
    }

    return static_cast<int>(msg.wParam);
}

bool D3DApp::Init()
{
    return InitMainWindow();
}

void D3DApp::OnResize()
{
}

void D3DApp::UpdateScene(float dt)
{
}

void D3DApp::DrawScene()
{
}

LRESULT D3DApp::MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_ACTIVATE:
    {
        if (LOWORD(wParam) == WA_INACTIVE)
        {
            mAppPaused = true;
            mTimer.Stop();
        }
        else
        {
            mAppPaused = false;
            mTimer.Start();
        }

        return 0;
    }

    case WM_ENTERSIZEMOVE:
    {
        mAppPaused = true;
        mResizing = true;
        mTimer.Stop();
        return 0;
    }

    case WM_EXITSIZEMOVE:
    {
        mAppPaused = false;
        mResizing = false;
        mTimer.Start();
        OnResize();
        return 0;
    }

    case WM_GETMINMAXINFO:
        reinterpret_cast<MINMAXINFO*>(lParam)->ptMinTrackSize.x = 300;
        reinterpret_cast<MINMAXINFO*>(lParam)->ptMinTrackSize.y = 200;
        return 0;

    case WM_MENUCHAR:
        // Don't beep when we alt-enter.
        return MAKELRESULT(0, MNC_CLOSE);

    case WM_LBUTTONDOWN:
    case WM_MBUTTONDOWN:
    case WM_RBUTTONDOWN:
        OnMouseDown(wParam, LOWORD(lParam), HIWORD(lParam));
        return 0;

    case WM_LBUTTONUP:
    case WM_MBUTTONUP:
    case WM_RBUTTONUP:
        OnMouseUp(wParam, LOWORD(lParam), HIWORD(lParam));
        return 0;
    case WM_MOUSEMOVE:
        OnMouseMove(wParam, LOWORD(lParam), HIWORD(lParam));
        return 0;

    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }

    return DefWindowProc(hWnd, msg, wParam, lParam);
}

LRESULT CALLBACK D3DApp::WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    const auto pApp = reinterpret_cast<D3DApp*>(GetWindowLongPtr(hWnd, GWLP_USERDATA));

    if (pApp)
    {
        return pApp->MsgProc(hWnd, msg, wParam, lParam);
    }

    return DefWindowProc(hWnd, msg, wParam, lParam);
}

bool D3DApp::InitMainWindow()
{
    WNDCLASS wc;

    wc.style = CS_HREDRAW | CS_VREDRAW;
    wc.lpfnWndProc = WndProc;
    wc.cbClsExtra = 0;
    wc.cbWndExtra = 0;
    wc.hInstance = mhAppInst;
    wc.hIcon = LoadIcon(nullptr, IDI_APPLICATION);
    wc.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wc.hbrBackground = static_cast<HBRUSH>(GetStockObject(WHITE_BRUSH));
    wc.lpszMenuName = nullptr;
    wc.lpszClassName = L"Sample";

    if (!RegisterClass(&wc))
    {
        MessageBox(nullptr, L"RegisterClass failed.", L"Native Sample", MB_ICONERROR);
        return false;
    }

    mhMainWnd = CreateWindow(
        L"Sample",
        L"Native Sample",
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT,
        CW_USEDEFAULT,
        CW_USEDEFAULT,
        CW_USEDEFAULT,
        nullptr,
        nullptr,
        mhAppInst,
        nullptr);

    if (mhMainWnd == nullptr)
    {
        MessageBox(nullptr, L"CreateWindow failed.", L"Native Sample", MB_ICONERROR);
        return false;
    }

    SetWindowLongPtr(mhMainWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(this));

    ShowWindow(mhMainWnd, SW_SHOWNORMAL);
    UpdateWindow(mhMainWnd);
}

bool D3DApp::InitDirect3D()
{
    return false;
}

void D3DApp::CalculateFrameStats() const
{
    static int frameCnt = 0;
    static float timeElapsed = 0.0f;
    frameCnt++;

    const float gameTime = mTimer.GameTime();

    if (gameTime - timeElapsed < 1.0f)
    {
        return;
    }

    const float fps = static_cast<float>(frameCnt);
    const float mspf = 1000.0f / fps;

    std::wostringstream outs;
    outs.precision(6);
    outs << mMainWndCaption << L" "
        << L"FPS: " << fps << L" "
        << L"Frame Time: " << mspf << L" (ms) "
        << L"Game Time: " << gameTime << L" (s)";

    SetWindowText(mhMainWnd, outs.str().c_str());

    frameCnt = 0;
    timeElapsed += 1.0f;
}
