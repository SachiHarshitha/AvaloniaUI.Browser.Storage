using Avalonia.OpenGL;

using AvaloniaUI.Browser.Storage.Contracts;

using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using AvaloniaUI.Browser.Storage.Models;

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

    [JSImport("saveFileWithMetadata", ModuleName)]
    internal static partial Task JsSaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key, string ase64String, string mimeType, string fileName, string createdAt, string lastModifiedAt, string fileFormat);

    [JSImport("getFileFromIndexedDBAsBase64", ModuleName)]
    internal static partial Task<string?> GetFileAsBase64Async(string dbName, int dbVersion, string storeName, string key);

    [JSImport("getFileWithMetadata", ModuleName)]
    internal static partial Task<JSObject?> JsGetFileWithMetadata(string dbName, int dbVersion, string storeName, string key);

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
    /// Save File with Metadata Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="mimeType"></param>
    /// <param name="fileName"></param>
    /// <param name="createdAt"></param>
    /// <param name="lastModifiedAt"></param>
    /// <param name="author"></param>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    public async Task SaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key, byte[] data, string mimeType,
        string fileName, string createdAt, string lastModifiedAt, string fileFormat)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        // Convert byte array to Base64 string
        var base64String = Convert.ToBase64String(data);
        await JsSaveFileWithMetadataAsync(
            dbName,
            dbVersion,
            storeName,
            key,
            base64String,
            mimeType,
            fileName,
            createdAt,
            lastModifiedAt,
            fileFormat);
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

    public async Task<FileWithMetadata?> GetFileWithMetadataAsync(
        string dbName,
        int dbVersion,
        string storeName,
        string key)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);

        var fileObject = await JsGetFileWithMetadata(dbName, dbVersion, storeName, key);

        if (fileObject is null)
            return null;

        Console.WriteLine(fileObject);
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsString("fileName")}");
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsString("mimeType")}");
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsDouble("size")}");
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsString("createdAt")}");
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsString("lastModifiedAt")}");
        Console.WriteLine($"Object has property:key:{fileObject.GetPropertyAsString("fileFormat")}");
        // Extract properties from JSObject
        var fileWithMetadata = new FileWithMetadata
        {
            Key = fileObject.GetPropertyAsString("key"),
            FileName = fileObject.GetPropertyAsString("fileName"),
            MimeType = fileObject.GetPropertyAsString("mimeType"),
            Size = fileObject.GetPropertyAsDouble("size"),
            CreatedAt = fileObject.GetPropertyAsString("createdAt"),
            LastModifiedAt = fileObject.GetPropertyAsString("lastModifiedAt"),
            FileFormat = fileObject.GetPropertyAsString("fileFormat")
        };

        var base64 = fileObject.GetPropertyAsString("dataBase64");
        if (!string.IsNullOrEmpty(base64))
        {
            fileWithMetadata.Data = Convert.FromBase64String(base64);
        }

        return fileWithMetadata;
    }

    #endregion Public Methods
}