#include "D3DRenderer.h"

D3DRenderer::D3DRenderer() :
    m4xMsaaQuality(0),
    md3dDevice(nullptr),
    md3dImmediateContext(nullptr),
    mSwapChain(nullptr),
    mDepthStencilBuffer(nullptr),
    mRenderTargetView(nullptr),
    mDepthStencilView(nullptr),
    mScreenViewport(),
    md3dDriverType(),
    mEnable4xMsaa(false)
{
}

D3DRenderer::~D3DRenderer()
{
}

bool D3DRenderer::Init()
{
    return true;
}
