using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Updatum;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private static readonly UpdatumManager AppUpdater = new("jonathan-carr031", "stoat-custom-desktop")
    {
        InstallUpdateWindowsExeType = UpdatumWindowsExeType.Installer,
        InstallUpdateWindowsInstallerArguments = "/qb" // Displays a basic user interface for MSI package
    };

    private readonly DispatcherTimer _timer = new();

    private readonly Panel? _webViewContainer;
    private NativeWebView? _nativeWebView;
    private DateTime _sessionExpirationDate = DateTime.Now;

    public MainWindow()
    {
        InitializeComponent();

        _nativeWebView = this.FindControl<NativeWebView>("StoatWebView");
        _webViewContainer = this.FindControl<Panel>("WebViewContainer");

        _timer.Interval = TimeSpan.FromSeconds(60);
        _timer.Tick += (s, e) => { _ = CheckSession(); };
        _timer.Start();
    }

    private void NavigationStarted_EventHandler(object? sender, WebViewNavigationStartingEventArgs e)
    {
        _ = CheckSession();
    }

    public void NavigationCompleted_EventHandler(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        _ = CheckForUpdates();
    }

    public void NewWindowRequested_EventHandler(object? sender, WebViewNewWindowRequestedEventArgs e)
    {
        if (e.Request == null ||
            string.IsNullOrWhiteSpace(e.Request.ToString())) return;

        e.Handled = true;

        var psi = new ProcessStartInfo
        {
            FileName = e.Request.ToString(),
            UseShellExecute = true
        };

        Process.Start(psi);
    }

    private async Task CheckSession()
    {
        var cookieManager = _nativeWebView?.TryGetCookieManager();
        if (cookieManager != null)
        {
            var cookies = await cookieManager.GetCookiesAsync();

            var authenticCookie = cookies.First(cookie => cookie.Name.StartsWith("authentik_proxy"));

            if (_sessionExpirationDate != authenticCookie.Expires)
            {
                _sessionExpirationDate = authenticCookie.Expires;
                ReCreateWebView();
            }
        }
    }

    private void ReCreateWebView()
    {
        _webViewContainer?.Children.Clear();

        var nativeWebView = new NativeWebView
        {
            Source = new Uri("https://chat.whalestargroup.com/app")
        };
        nativeWebView.NavigationStarted += NavigationStarted_EventHandler;
        nativeWebView.NavigationCompleted += NavigationCompleted_EventHandler;
        nativeWebView.NewWindowRequested += NewWindowRequested_EventHandler;
        nativeWebView.Focusable = false;

        _webViewContainer?.Children.Add(nativeWebView);

        _nativeWebView = nativeWebView;
    }

    private async Task CheckForUpdates()
    {
        try
        {
            var updateFound = await AppUpdater.CheckForUpdatesAsync();
            if (!updateFound) return;

            var downloadedAsset = await AppUpdater.DownloadUpdateAsync();

            if (downloadedAsset == null) return;
#if !DEBUG
            await AppUpdater.InstallUpdateAsync(downloadedAsset);
#endif
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}