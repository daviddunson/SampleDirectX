#include "Common.h"
#include "D3DApp.h"

int WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ PSTR szCmdLine, _In_ int iCmdShow)
{
	D3DApp app(hInstance);
	app.Init();
	app.Run();
}
