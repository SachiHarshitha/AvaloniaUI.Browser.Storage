using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using AvaloniaUI.Browser.Storage.Contracts;

namespace AvaloniaUI.Browser.Storage;

public partial class IndexedDbFileService : IIndexedDbFileService
{
    #region Fields

    private const string ModuleName = "FuncIndexedDbFile";

    private const string ModulePath = "/js/FuncIndexedDbFile.js";

    private JSObject? _indexedDbModule;

    #endregion Fields

    public IndexedDbFileService()
    {
        _ = InitializeAsync();
    }

    #region JSInterop

    [JSImport("saveFileToIndexedDBFromBase64", ModuleName)]
    internal static partial Task SaveFileFromBase64Async(string dbName, int dbVersion, string storeName, string key, string base64String, string mimeType);

    [JSImport("getFileFromIndexedDBAsBase64", ModuleName)]
    internal static partial Task<string?> GetFileAsBase64Async(string dbName, int dbVersion, string storeName, string key);

    #endregion JSInterop

    #region Public Methods

    /// <summary>
    /// Initialization method
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAsync()
    {
        _indexedDbModule = await JSHost.ImportAsync(ModuleName, ModulePath);
    }

    /// <summary>
    /// Save File Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    public async Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data, string mimeType)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        // Convert byte array to Base64 string
        var base64String = Convert.ToBase64String(data);
        await SaveFileFromBase64Async(dbName, dbVersion, storeName, key, base64String, mimeType);
    }

    /// <summary>
    /// Get File Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<byte[]?> GetFileAsync(string dbName, int dbVersion, string storeName, string key)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);

        var base64 = await GetFileAsBase64Async(dbName, dbVersion, storeName, key);
        if (string.IsNullOrEmpty(base64))
            return null;

        return Convert.FromBase64String(base64);
    }

    #endregion Public Methods
}