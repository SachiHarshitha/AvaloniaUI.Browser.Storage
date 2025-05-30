using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using AvaloniaUI.Browser.Storage.Contracts;

namespace AvaloniaUI.Browser.Storage;

public partial class IndexedDbFileService : IIndexedDbFileService
{
    private static Lazy<Task<JSObject>> _module = new(() =>
        JSHost.ImportAsync("FuncIndexedDbFile ", "./js/FuncIndexedDbFile.js"));

    [JSImport("saveFileToIndexedDBFromBase64", "FuncIndexedDbFile")]
    internal static partial Task SaveFileFromBase64Async(string dbName, int dbVersion, string storeName, string key, string base64String, string mimeType);

    [JSImport("getFileFromIndexedDBAsBase64", "FuncIndexedDbFile")]
    internal static partial Task<string?> GetFileAsBase64Async(string dbName, int dbVersion, string storeName, string key);

    public async Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data, string mimeType)
    {
        // Convert byte array to Base64 string
        var base64String = Convert.ToBase64String(data);
        await SaveFileFromBase64Async(dbName, dbVersion, storeName, key, base64String, mimeType);
    }

    public async Task<byte[]?> GetFileAsync(string dbName, int dbVersion, string storeName, string key)
    {
        var base64 = await GetFileAsBase64Async(dbName, dbVersion, storeName, key);
        if (string.IsNullOrEmpty(base64))
            return null;

        return Convert.FromBase64String(base64);
    }
}