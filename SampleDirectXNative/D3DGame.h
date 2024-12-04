#pragma once
#include "Common.h"
#include "D3DRenderer.h"
#include "D3DWindow.h"
#include "GameTimer.h"

class D3DGame : public D3DWindow
{
public:

    D3DGame(HINSTANCE hInstance);
    virtual ~D3DGame();

    bool Init() override;
    void Tick();

protected:

    void OnResizing() override;
    void OnResize() override;
    void OnActivated() override;
    void OnDeactivated() override;

    virtual void UpdateScene(float dt) {}
    virtual void DrawScene() {}

    void CalculateFrameStats() const;

    D3DRenderer mRenderer;
    GameTimer mTimer;
    bool mAppPaused;
};

