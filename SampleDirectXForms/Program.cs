namespace SampleDirectXForms;

using System;
using System.Runtime.InteropServices;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new D3DApp(Marshal.GetHINSTANCE(typeof(Program).Module));
        app.Init();
        app.Run();
    }
}