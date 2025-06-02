using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaWASM.Storage.Contracts;

namespace AvaloniaUI.Browser.Storage.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
    }

    /// <summary>
    /// Default constructor for MainViewModel.
    /// </summary>
    public MainViewModel(IIndexedDbFileService indexedDbFileService, ISessionStorageService sessionStorageService,
        ILocalStorageService localStorageService)
    {
        // Initialize the session storage service and local storage service.
        _sessionStorageService = sessionStorageService;
        _localStorageService = localStorageService;
        _indexedDbFileService = indexedDbFileService;
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

    /// <summary>
    /// Stores a value in session storage and retrieves it.
    /// </summary>
    [RelayCommand]
    public async Task SessionStorageCommand()
    {
        var guid = Guid.NewGuid().ToString();
        await _sessionStorageService.SetItemAsync(guid, ValueToSetSessionStorage);
        ValueFromSessionStorage = await _sessionStorageService.GetItemAsync(guid);
        GetSessionStorageEntries();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Stores a value in local storage and retrieves it.
    /// </summary>
    [RelayCommand]
    public async Task LocalStorageCommand()
    {
        var guid = Guid.NewGuid().ToString();
        await _localStorageService.SetItemAsync(guid, ValueToSetLocalStorage);
        ValueFromLocalStorage = await _localStorageService.GetItemAsync(guid);
        GetLocalStorageEntries();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Clear the session storage.
    /// </summary>
    [RelayCommand]
    public async Task ClearSessionStorageAsync()
    {
        await _sessionStorageService.ClearAsync();
        ValueFromSessionStorage = string.Empty;
        SessionStorageEntries.Clear();
    }

    /// <summary>
    /// Clear the local storage.
    /// </summary>
    [RelayCommand]
    public async Task ClearLocalStorageAsync()
    {
        await _localStorageService.ClearAsync();

        ValueFromLocalStorage = string.Empty;
        LocalStorageEntries.Clear();
    }

    /// <summary>
    /// Browse a file using the file picker dialog.
    /// </summary>
    [RelayCommand]
    public async Task BrowseFile()
    {
        LoadedFileContent = string.Empty; // Reset the content before storing a new file
        LoadedFileMetadata = string.Empty; // Reset the metadata before loading a new file
        // Start async operation to open the dialog.
        var files = await App.TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            _file = files[0] as IStorageFile;
            var props = await _file.GetBasicPropertiesAsync();
            LoadedFileMetadata =
                $"Name: {_file.Name}, Size: {props.Size} bytes, Created On: {props.DateCreated}, Modified On: {props.DateModified}";
            await using var stream = await files[0].OpenReadAsync();
            _fileContent = await ReadFully(stream);
            LoadedFileContent = Encoding.UTF8.GetString(_fileContent);
        }
    }

    /// <summary>
    /// Query IndexedDB for all file entries.
    /// </summary>
    [RelayCommand]
    public async Task QueryIndexedDB()
    {
        IndexedDbEntries.Clear();
        var entries = await _indexedDbFileService.GetAllFileEntriesAsync(dbName, 2, storeName);
        if (entries != null)
            foreach (var entry in entries)
                IndexedDbEntries.Add(new Tuple<string, object>(entry.Key, entry.ToString()));
    }

    /// <summary>
    /// Save the file to IndexedDB with metadata.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [RelayCommand]
    public async Task StoreFile()
    {
        DatabaseFileContent = string.Empty; // Reset the content before storing a new file

        if (!(_fileContent?.Length > 0))
        {
            throw new InvalidOperationException("File content is empty. Please browse a file first.");
        }

        var props = await _file.GetBasicPropertiesAsync();
        var guid = Guid.NewGuid().ToString();
        await _indexedDbFileService.SaveFileWithMetadataAsync(dbName, 2, storeName, guid, _fileContent,
            MimeTypes.TextPlain, _file.Name, props.DateCreated?.ToString(), props.DateModified?.ToString(),
            _file.Name.Split('.').Last());

        var dbFile = await _indexedDbFileService.GetFileWithMetadataAsync(dbName, 2, storeName, guid);
        if (dbFile == null)
        {
            throw new InvalidOperationException("Failed to load file from IndexedDB.");
        }

        DatabaseFileContent = Encoding.UTF8.GetString(dbFile.Data);
        DatabaseFileMetaData =
            $"Name: {dbFile.FileName}, Size: {dbFile.Data.Length} bytes, Created On: {dbFile.CreatedAt}, Modified On: {dbFile.LastModifiedAt}, Format: {dbFile.FileFormat}";
        // Reload the IndexedDB entries to reflect the new file.
        await QueryIndexedDB();
    }

    /// <summary>
    /// Get Session Storage Entries Command.
    /// </summary>
    [RelayCommand]
    public async void GetSessionStorageEntries()
    {
        try
        {
            SessionStorageEntries.Clear();

            var length = await _sessionStorageService.GetLength();

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

    /// <summary>
    /// Get Local Storage Entries Command.
    /// </summary>
    [RelayCommand]
    public async void GetLocalStorageEntries()
    {
        try
        {
            LocalStorageEntries.Clear();

            var length = await _localStorageService.GetLength();

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

    /// <summary>
    /// Read the entire stream into a byte array asynchronously.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

    #region Fields

    private string dbName = "TestDB";
    private string storeName = "Test_Store";
    private byte[] _fileContent;
    private IStorageFile _file;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly ILocalStorageService _localStorageService;
    private readonly IIndexedDbFileService _indexedDbFileService;

    [ObservableProperty] private string _valueToSetLocalStorage = string.Empty;

    [ObservableProperty] private string _valueToSetSessionStorage = string.Empty;

    [ObservableProperty] private string _valueFromSessionStorage = string.Empty;

    [ObservableProperty] private string _valueFromLocalStorage = string.Empty;

    [ObservableProperty] private string _loadedFileContent = string.Empty;

    [ObservableProperty] private string _databaseFileContent = string.Empty;

    [ObservableProperty] private string _loadedFileMetadata = string.Empty;

    [ObservableProperty] private string _databaseFileMetaData = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Tuple<string, object>> _sessionStorageEntries =
        new ObservableCollection<Tuple<string, object>>();

    [ObservableProperty]
    private ObservableCollection<Tuple<string, object>> _localStorageEntries =
        new ObservableCollection<Tuple<string, object>>();

    [ObservableProperty]
    private ObservableCollection<Tuple<string, object>> _indexedDbEntries =
        new ObservableCollection<Tuple<string, object>>();

    #endregion Fields
}