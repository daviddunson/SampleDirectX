namespace SampleDirectXForms;

using static NativeMethods;

internal static class D3DApp
{
    public static void Run()
    {
        while (GetMessage(out var msg, nint.Zero, 0, 0) != 0)
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }

    public static int Run(D3DGame game)
    {
        game.Init();

        var msg = new NativeMessage();

        while (msg.msg != WM_QUIT)
        {
            if (PeekMessage(out msg, nint.Zero, 0, 0, (uint)PeekMessageParams.PM_REMOVE))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            else
            {
                game.Tick();
            }
        }

        return msg.wParam.ToInt32();
    }
}