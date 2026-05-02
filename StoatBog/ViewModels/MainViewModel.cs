using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";

    public DateTime SessionExpirationDate = DateTime.Now;
    public string SessionExpirationDateString => SessionExpirationDate.ToString("MM/dd/yyyy HH:mm tt");
    public bool IsSessionExpired => SessionExpirationDate > DateTime.Now;
}