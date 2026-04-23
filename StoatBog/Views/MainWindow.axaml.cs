using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private readonly NativeWebView? _nativeWebView;

    public MainWindow()
    {
        InitializeComponent();

        _nativeWebView = this.FindControl<NativeWebView>("StoatWebView");
    }

    private void NavigationStarted_EventHandler(object? sender, WebViewNavigationStartingEventArgs e)
    {
        Console.WriteLine("NavigationStarted_EventHandler");

        _ = CheckSession();

        Console.WriteLine(sender);
        Console.WriteLine(e);
    }

    public void NavigationCompleted_EventHandler(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        Console.WriteLine("NavigationCompleted_EventHandler");
        Console.WriteLine(sender);
        Console.WriteLine(e);
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

            foreach (var cookie in cookies)
            {
                if (cookie.Expires < DateTime.UtcNow || cookie.Expired)
                    Debug.WriteLine($"{cookie.Name} is Expired. It expired on {cookie.Expires.ToLocalTime()}");

                Debug.WriteLine($"{cookie.Name}: {cookie.Value} - Expires: {cookie.Expires}");
            }

            Console.WriteLine(cookies.First(c => c.Name == "session_id").Value);
        }
    }
}