using System;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaApplication1;
using Updatum;

namespace StoatBog.Desktop;

internal sealed class Program
{
    private static readonly UpdatumManager AppUpdater = new("jonathan-carr031", "stoat-custom-desktop")
    {
        InstallUpdateWindowsInstallerArguments = "/qb" // Displays a basic user interface for MSI package
    };

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // _ = CheckForUpdates();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }


    // Avalonia configuration, don't remove; also used by visual designer. 
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static async Task CheckForUpdates()
    {
        try
        {
            var updateFound = await AppUpdater.CheckForUpdatesAsync();
            if (!updateFound) return;

            // Optional show a message to the user with the changelog
            Console.WriteLine("Changelog:");
            Console.WriteLine(AppUpdater.GetChangelog());

            var downloadedAsset = await AppUpdater.DownloadUpdateAsync();

            if (downloadedAsset == null)
            {
                Console.WriteLine("Failed to download the update.");
                return;
            }

            // You can manually handle the installation or call the installation method:
            // Returns false if the installation failed, otherwise it will never return true as the process will be terminated to complete the installation.
            await AppUpdater.InstallUpdateAsync(downloadedAsset);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}