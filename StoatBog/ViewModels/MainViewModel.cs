using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private DateTime _sessionExpirationDate;


    public DateTime SessionExpirationDate
    {
        get => _sessionExpirationDate;
        set
        {
            _sessionExpirationDate = value;
            OnPropertyChanged(nameof(SessionExpirationDateString));
            OnPropertyChanged(nameof(IsSessionExpired));
        }
    }

    public string SessionExpirationDateString => SessionExpirationDate.ToString("MM/dd/yyyy hh:mm tt");
    public bool IsSessionExpired => SessionExpirationDate < DateTime.Now;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}