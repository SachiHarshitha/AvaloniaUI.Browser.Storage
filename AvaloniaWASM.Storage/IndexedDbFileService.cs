using AvaloniaWASM.Storage.Contracts;
using AvaloniaWASM.Storage.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaWASM.Storage;

public partial class IndexedDbFileService : IIndexedDbFileService
{
    /// <summary>
    /// Defaultl constructor for IndexedDbFileService
    /// </summary>
    public IndexedDbFileService()
    {
        _ = InitializeAsync();
    }

    #region Fields

    private const string ModuleName = "FuncIndexedDbFile";

    private const string ModulePath = "/js/FuncIndexedDbFile.js";

    private JSObject? _indexedDbModule;

    #endregion Fields

    #region JSInterop

    [JSImport("saveFileToIndexedDBFromBase64", ModuleName)]
    internal static partial Task SaveFileFromBase64Async(string dbName, int dbVersion, string storeName, string key,
        string base64String, string mimeType);

    [JSImport("saveFileWithMetadata", ModuleName)]
    internal static partial Task JsSaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key,
        string ase64String, string mimeType, string fileName, string createdAt, string lastModifiedAt,
        string fileFormat);

    [JSImport("getFileFromIndexedDBAsBase64", ModuleName)]
    internal static partial Task<string?> GetFileAsBase64Async(string dbName, int dbVersion, string storeName,
        string key);

    [JSImport("getFileWithMetadata", ModuleName)]
    internal static partial Task<JSObject?> JsGetFileWithMetadata(string dbName, int dbVersion, string storeName,
        string key);

    [JSImport("getAllKeysFromIndexedDB", ModuleName)]
    internal static partial Task<string?> GetAllKeysFromIndexedDBAsync(string dbName, int dbVersion, string storeName);

    [JSImport("getAllFileEntriesFromIndexedDB", "FuncIndexedDbFile")]
    internal static partial Task<string?> GetAllFileEntriesFromIndexedDBJsonAsync(string dbName, int dbVersion,
        string storeName);

    [JSImport("clearStore", ModuleName)]
    internal static partial Task<bool> JsClearStore(string dbName, string storeName, int dbVersion);

    [JSImport("deleteDatabase", ModuleName)]
    internal static partial Task<bool> JsDeleteDatabase(string dbName);

    [JSImport("clearAllStores", ModuleName)]
    internal static partial Task<bool> JsClearAllStores(string dbName, int dbVersion);

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
    public async Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType)
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
    public async Task SaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType,
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

    /// <summary>
    /// Get File with Metadata Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get All Keys Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public async Task<string[]?> GetAllKeysAsync(string dbName, int dbVersion, string storeName)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);

        var json = await GetAllKeysFromIndexedDBAsync(dbName, dbVersion, storeName);

        return string.IsNullOrEmpty(json)
            ? null
            :
            // Deserialize JSON array into string[]
            JsonSerializer.Deserialize<string[]>(json);
    }

    /// <summary>
    /// Get All File Entries Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public async Task<List<FileWithMetadata>?> GetAllFileEntriesAsync(string dbName, int dbVersion, string storeName)
    {
        _indexedDbModule ??= await JSHost.ImportAsync("FuncIndexedDbFile", "/js/FuncIndexedDbFile.js");

        var json = await GetAllFileEntriesFromIndexedDBJsonAsync(dbName, dbVersion, storeName);

        if (string.IsNullOrEmpty(json))
            return null;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var items = JsonSerializer.Deserialize<List<FileWithMetadata>>(json, options);
        // Deserialize JSON to List<FileWithMetadata>
        return items;
    }

    /// <summary>
    /// Clear Store Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="storeName"></param>
    /// <param name="dbVersion"></param>
    /// <returns></returns>
    public async Task<bool> ClearStoreAsync(string dbName, string storeName, int dbVersion)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsClearStore(dbName, storeName, dbVersion);
    }

    /// <summary>
    /// Clear All Stores Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <returns></returns>
    public async Task<bool> ClearAllStoresAsync(string dbName, int dbVersion)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsClearAllStores(dbName, dbVersion);
    }

    /// <summary>
    /// Delete Database Async Method
    /// </summary>
    /// <param name="dbName"></param>
    /// <returns></returns>
    public async Task<bool> DeleteDatabaseAsync(string dbName)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsDeleteDatabase(dbName);
    }

    #endregion Public Methods
}