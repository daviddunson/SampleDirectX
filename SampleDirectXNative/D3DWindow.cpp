#include "D3DWindow.h"

D3DWindow::D3DWindow(HINSTANCE hInstance) :
    mhAppInst(hInstance),
    mhMainWnd(nullptr),
    mMinimized(false),
    mMaximized(false),
    mResizing(false),
    mClientWidth(0),
    mClientHeight(0)
{
}

D3DWindow::~D3DWindow()
{
}

float D3DWindow::AspectRatio() const
{
    return static_cast<float>(mClientWidth) / mClientHeight;
}

HINSTANCE D3DWindow::AppInst() const
{
    return mhAppInst;
}

HWND D3DWindow::MainWnd() const
{
    return mhMainWnd;
}

bool D3DWindow::Init()
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

    return true;
}

LRESULT D3DWindow::MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_ACTIVATE:
    {
        if (LOWORD(wParam) == WA_INACTIVE)
        {
            OnDeactivated();
        }
        else
        {
            OnActivated();
        }

        return 0;
    }

    case WM_ENTERSIZEMOVE:
    {
        mResizing = true;
        OnResizing();
        return 0;
    }

    case WM_EXITSIZEMOVE:
    {
        mResizing = false;
        OnResize();
        return 0;
    }

    case WM_SIZE:
    {
        mClientWidth = static_cast<int>(lParam & 0xFFFF);
        mClientHeight = static_cast<int>((lParam >> 16) & 0xFFFF);
        break;
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

LRESULT CALLBACK D3DWindow::WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    const auto pApp = reinterpret_cast<D3DWindow*>(GetWindowLongPtr(hWnd, GWLP_USERDATA));

    if (pApp)
    {
        return pApp->MsgProc(hWnd, msg, wParam, lParam);
    }

    return DefWindowProc(hWnd, msg, wParam, lParam);
}
