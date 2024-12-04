#pragma once
#include "Common.h"

class D3DWindow
{
public:

    D3DWindow(HINSTANCE hInstance);
    virtual ~D3DWindow();

    HINSTANCE AppInst() const;
    HWND MainWnd() const;
    float AspectRatio() const;

    virtual bool Init();

protected:

    static LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    virtual LRESULT MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

    virtual void OnResizing() {}
    virtual void OnResize() {}
    virtual void OnActivated() {}
    virtual void OnDeactivated() {}
    virtual void OnMouseDown(WPARAM btnState, int x, int y) {}
    virtual void OnMouseUp(WPARAM btnState, int x, int y) {}
    virtual void OnMouseMove(WPARAM btnState, int x, int y) {}

    HINSTANCE mhAppInst;
    HWND mhMainWnd;
    std::wstring mMainWndCaption;
    bool mMinimized;
    bool mMaximized;
    bool mResizing;
    int mClientWidth;
    int mClientHeight;
};

