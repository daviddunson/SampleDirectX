namespace SampleDirectXForms;

using System;
using System.Runtime.InteropServices;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var instanceHandle = Marshal.GetHINSTANCE(typeof(Program).Module);
        var game = new D3DGame(instanceHandle);

        D3DApp.Run(game);
    }
}