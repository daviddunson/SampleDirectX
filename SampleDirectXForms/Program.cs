using System;

namespace SampleDirectXForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var window = new NativeWindow();
            window.Create();
            window.Run();
        }
    }
}