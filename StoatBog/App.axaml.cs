using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using ReactiveUI;

namespace AvaloniaApplication1;

public partial class App : Application
{
    private readonly Uri _iconUri = new("avares://StoatBog/Assets/Images/icon.ico");

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        DataContext = new MainViewModel();

        SetIcons();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow.Closing += (s, e) =>
            {
                if (e.CloseReason == WindowCloseReason.ApplicationShutdown) return;

                SetIcons();

                e.Cancel = true;
                (s as MainWindow)?.Hide();
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory =
                () => new MainView { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowWindow()
    {
        var window = Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (window is not { MainWindow: not null }) return;

        window.MainWindow.Show();
        window.MainWindow.WindowState = WindowState.Normal;
        window.MainWindow.ShowInTaskbar = true;
        window.MainWindow.Activate();

        ClearTrayIcons();
    }

    private static void QuitApplication()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void SetIcons()
    {
        var clickCommand = ReactiveCommand.Create(ShowWindow);
        var quitCommand = ReactiveCommand.Create(QuitApplication);

        var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        var icons = new TrayIcons
        {
            new TrayIcon
            {
                Icon = new WindowIcon(new Bitmap(AssetLoader.Open(_iconUri))),
                ToolTipText = "Stoat",
                Command = clickCommand,
                Menu =
                [
                    new NativeMenuItem("About")
                    {
                        Menu =
                        [
                            new NativeMenuItem($"v{version}")
                        ]
                    },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Quit Application")
                    {
                        Command = quitCommand
                    }
                ]
            }
        };

        if (Current != null) TrayIcon.SetIcons(Current, icons);
    }

    private void ClearTrayIcons()
    {
        var clickCommand = ReactiveCommand.Create(ShowWindow);
        var quitCommand = ReactiveCommand.Create(QuitApplication);

        var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        var icons = new TrayIcons
        {
            new TrayIcon
            {
                Icon = new WindowIcon(new Bitmap(AssetLoader.Open(_iconUri))),
                ToolTipText = "Stoat",
                Command = clickCommand,
                Menu =
                [
                    new NativeMenuItem("About")
                    {
                        Menu =
                        [
                            new NativeMenuItem($"v{version}")
                        ]
                    },
                    new NativeMenuItemSeparator(),
                    new NativeMenuItem("Quit Application")
                    {
                        Command = quitCommand
                    }
                ]
            }
        };

        if (Current != null) TrayIcon.SetIcons(Current, icons);
    }
}