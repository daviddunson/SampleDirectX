namespace SampleDirectXForms;

using System;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var window = new NativeWindow();
        window.Create();
        window.Run();
    }
}