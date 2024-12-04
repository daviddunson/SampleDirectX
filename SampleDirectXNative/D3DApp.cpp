#include "D3DApp.h"

int D3DApp::Run(D3DGame game)
{
    game.Init();

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
            game.Tick();
        }
    }

    return static_cast<int>(msg.wParam);
}
