using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Fields

    private readonly SessionStorageService _sessionStorageService;
    private readonly LocalStorageService _localStorageService;

    [ObservableProperty]
    private string _valueToSetLocalStorage = string.Empty;

    [ObservableProperty]
    private string _valueToSetSessionStorage = string.Empty;

    [ObservableProperty]
    private string _valueFromSessionStorage = string.Empty;

    [ObservableProperty]
    private string _valueFromLocalStorage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Tuple<string, object>> _sessionStorageEntries = new ObservableCollection<Tuple<string, object>>();

    [ObservableProperty]
    private ObservableCollection<Tuple<string, object>> _localStorageEntries = new ObservableCollection<Tuple<string, object>>();

    #endregion Fields

    /// <summary>
    /// Default constructor for MainViewModel.
    /// </summary>
    public MainViewModel()
    {
        // Initialize the session storage service and local storage service.
        _sessionStorageService = new SessionStorageService();
        _localStorageService = new LocalStorageService();
        // Optionally, you can load initial data or perform setup here.
        LoadInitialData();
    }

    #region Props

    public string Greeting => "Welcome to AvaloniaUI Browser Storage Library";

    #endregion Props

    /// <summary>
    /// Loads initial data from session and local storage.
    /// </summary>
    private void LoadInitialData()
    {
        GetSessionStorageEntries();
        GetLocalStorageEntries();
    }

    [RelayCommand]
    public async Task SessionStorageCommand()
    {
        var guid = Guid.NewGuid().ToString();
        await _sessionStorageService.SetItemAsync(guid, ValueToSetSessionStorage);
        ValueFromSessionStorage = await _sessionStorageService.GetItemAsync(guid);
        GetSessionStorageEntries();
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task LocalStorageCommand()
    {
        var guid = Guid.NewGuid().ToString();
        await _localStorageService.SetItemAsync(guid, ValueToSetLocalStorage);
        ValueFromLocalStorage = await _localStorageService.GetItemAsync(guid);
        GetLocalStorageEntries();
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ClearSessionStorageAsync()
    {
        await _sessionStorageService.ClearAsync();
        ValueFromSessionStorage = string.Empty;
        SessionStorageEntries.Clear();
    }

    [RelayCommand]
    public async Task ClearLocalStorageAsync()
    {
        await _localStorageService.ClearAsync();

        ValueFromLocalStorage = string.Empty;
        LocalStorageEntries.Clear();
    }

    [RelayCommand]
    public async void GetSessionStorageEntries()
    {
        try
        {
            SessionStorageEntries.Clear();

            var length = await _sessionStorageService.Length;

            for (var i = 0; i < length; i++)
            {
                var key = await _sessionStorageService.KeyAsync(i);
                if (string.IsNullOrEmpty(key)) continue;
                var value = await _sessionStorageService.GetItemAsync(key);
                SessionStorageEntries.Add(new Tuple<string, object>(key, value ?? "null"));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving session storage entries: {e.Message}");
        }
    }

    [RelayCommand]
    public async void GetLocalStorageEntries()
    {
        try
        {
            LocalStorageEntries.Clear();

            var length = await _localStorageService.Length;

            for (var i = 0; i < length; i++)
            {
                var key = await _localStorageService.KeyAsync(i);
                if (string.IsNullOrEmpty(key)) continue;
                var value = await _localStorageService.GetItemAsync(key);
                LocalStorageEntries.Add(new Tuple<string, object>(key, value ?? "null"));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving local storage entries: {e.Message}");
        }
    }
}