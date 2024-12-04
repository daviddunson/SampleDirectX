#pragma once
#include "Common.h"

class D3DRenderer
{
public:

    D3DRenderer();
    virtual ~D3DRenderer();

    bool Init();

protected:

    UINT m4xMsaaQuality;
    ID3D11Device* md3dDevice;
    ID3D11DeviceContext* md3dImmediateContext;
    IDXGISwapChain* mSwapChain;
    ID3D11Texture2D* mDepthStencilBuffer;
    ID3D11RenderTargetView* mRenderTargetView;
    ID3D11DepthStencilView* mDepthStencilView;
    D3D11_VIEWPORT mScreenViewport;
    D3D_DRIVER_TYPE md3dDriverType;
    bool mEnable4xMsaa;

};

