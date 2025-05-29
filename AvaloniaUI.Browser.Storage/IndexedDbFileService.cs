using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage;

public partial class IndexedDbFileService
{
    private static Lazy<Task<JSObject>> _module = new(() =>
        JSHost.ImportAsync("FileIndexedDB", "./js/FuncIndexedDbFile.js"));

    [JSImport("saveFileToIndexedDBFromBase64", "FileIndexedDB")]
    internal static partial Task SaveFileFromBase64Async(string dbName, string storeName, string key, string base64String, string mimeType);

    public static async Task SaveFileAsync(string dbName, string storeName, string key, byte[] data, string mimeType)
    {
        // Convert byte array to Base64 string
        var base64String = Convert.ToBase64String(data);
        await SaveFileFromBase64Async(dbName, storeName, key, base64String, mimeType);
    }

    [JSImport("getFileFromIndexedDBAsBase64", "FileIndexedDB")]
    internal static partial Task<string?> GetFileAsBase64Async(string dbName, string storeName, string key);

    public static async Task<byte[]?> LoadFileAsBase64Async(string dbName, string storeName, string key)
    {
        var base64 = await GetFileAsBase64Async(dbName, storeName, key);
        if (string.IsNullOrEmpty(base64))
            return null;

        return Convert.FromBase64String(base64);
    }
}