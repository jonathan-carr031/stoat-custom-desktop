using System;
using System.Threading;
using Avalonia;
using AvaloniaApplication1;

namespace StoatBog.Desktop;

internal sealed class Program
{
    private static readonly Mutex Mutex = new(true, "A6669AD0-DB69-4340-96ED-2CFBF04AA35D");


    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (!Mutex.WaitOne(TimeSpan.Zero, true))
            // Another instance is already running
            return;


        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        GC.KeepAlive(Mutex);
    }


    // Avalonia configuration, don't remove; also used by visual designer. 
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}