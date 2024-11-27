#pragma once
#include "Common.h"
#include "GameTimer.h"

class D3DApp
{
public:

    D3DApp(HINSTANCE hInstance);
    virtual ~D3DApp();

    HINSTANCE AppInst() const;
    HWND MainWnd() const;
    float AspectRatio() const;
    int Run();

    virtual bool Init();
    virtual void OnResize();
    virtual void UpdateScene(float dt);
    virtual void DrawScene();
    virtual LRESULT MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

    virtual void OnMouseDown(WPARAM btnState, int x, int y) {}
    virtual void OnMouseUp(WPARAM btnState, int x, int y) {}
    virtual void OnMouseMove(WPARAM btnState, int x, int y) {}

protected:

    static LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

    bool InitMainWindow();
    bool InitDirect3D();
    void CalculateFrameStats() const;

    HINSTANCE mhAppInst;
    HWND mhMainWnd;
    bool mAppPaused;
    bool mMinimized;
    bool mMaximized;
    bool mResizing;
    UINT m4xMsaaQuality;

    GameTimer mTimer;

    ID3D11Device* md3dDevice;
    ID3D11DeviceContext* md3dImmediateContext;
    IDXGISwapChain* mSwapChain;
    ID3D11Texture2D* mDepthStencilBuffer;
    ID3D11RenderTargetView* mRenderTargetView;
    ID3D11DepthStencilView* mDepthStencilView;
    D3D11_VIEWPORT mScreenViewport;

    std::wstring mMainWndCaption;

    D3D_DRIVER_TYPE md3dDriverType;

    int mClientWidth;
    int mClientHeight;

    bool mEnable4xMsaa;
};

