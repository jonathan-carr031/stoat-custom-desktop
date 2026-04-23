using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private readonly Panel? _webViewContainer;
    private NativeWebView? _nativeWebView;
    private DateTime _sessionExpirationDate = DateTime.Now;


    public MainWindow()
    {
        InitializeComponent();

        _nativeWebView = this.FindControl<NativeWebView>("StoatWebView");
        _webViewContainer = this.FindControl<Panel>("WebViewContainer");
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

            var authenticCookie = cookies.First(cookie => cookie.Name == "authentik_proxy_72cd35ff");

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
}