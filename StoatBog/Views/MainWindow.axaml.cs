using System;
using Avalonia.Controls;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void NewWindowRequested_EventHandler(object? sender,
        WebViewNewWindowRequestedEventArgs webViewNewWindowRequestedEventArgs)
    {
        if (webViewNewWindowRequestedEventArgs.Request == null ||
            string.IsNullOrWhiteSpace(webViewNewWindowRequestedEventArgs.Request.ToString())) return;

        webViewNewWindowRequestedEventArgs.Handled = true;

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = webViewNewWindowRequestedEventArgs.Request.ToString(),
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
    }
}