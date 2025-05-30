using Avalonia.Controls;
using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Fields

    private byte[] _fileContent;
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
    private string _loadedFileContent = string.Empty;

    [ObservableProperty]
    private string _databaseFileContent = string.Empty;

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
    public async Task BrowseFile()
    {
        LoadedFileContent = string.Empty; // Reset the content before storing a new file

        // Start async operation to open the dialog.
        var files = await App.TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            _fileContent = await ReadFully(stream);
            LoadedFileContent = System.Text.Encoding.UTF8.GetString(_fileContent);
        }
    }

    [RelayCommand]
    public async Task StoreFile()
    {
        DatabaseFileContent = string.Empty; // Reset the content before storing a new file
        var dbName = "TestDB";
        var storeName = "Test_Store";
        var fileName = "Test_File";

        if (!(_fileContent?.Length > 0))
        {
            throw new InvalidOperationException("File content is empty. Please browse a file first.");
            return;
        }

        await IndexedDbFileService.SaveFileAsync(dbName, storeName, fileName, _fileContent, "text/plain");

        var loadedFile = await IndexedDbFileService.LoadFileAsBase64Async(dbName, storeName, fileName);
        if (loadedFile == null)
        {
            throw new InvalidOperationException("Failed to load file from IndexedDB.");
        }
        DatabaseFileContent = System.Text.Encoding.UTF8.GetString(loadedFile);
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

    public static async Task<byte[]> ReadFully(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await ms.WriteAsync(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}