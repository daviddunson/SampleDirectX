#include "D3DGame.h"

D3DGame::D3DGame(HINSTANCE hInstance) :
    D3DWindow(hInstance),
    mAppPaused(false)
{
}

D3DGame::~D3DGame()
{
}

bool D3DGame::Init()
{
    D3DWindow::Init();
    mRenderer.Init();
    mTimer.Reset();

    return true;
}

void D3DGame::Tick()
{
    mTimer.Tick();

    if (mAppPaused)
    {
        Sleep(100);
        return;
    }
    
    CalculateFrameStats();
    UpdateScene(mTimer.DeltaTime());
    DrawScene();

    Sleep(0);
}

void D3DGame::OnActivated()
{
    mAppPaused = false;
    mTimer.Start();
}

void D3DGame::OnDeactivated()
{
    mAppPaused = true;
    mTimer.Stop();
}

void D3DGame::OnResize()
{
    mAppPaused = false;
    mTimer.Start();
}

void D3DGame::OnResizing()
{
    mAppPaused = true;
    mTimer.Stop();
}

void D3DGame::CalculateFrameStats() const
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
