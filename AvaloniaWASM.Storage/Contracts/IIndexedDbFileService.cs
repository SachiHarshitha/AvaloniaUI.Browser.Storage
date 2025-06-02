using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaWASM.Storage.Models;

namespace AvaloniaWASM.Storage.Contracts;

public interface IIndexedDbFileService
{
    /// <summary>
    /// Get all keys from the IndexedDB store
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public Task<string[]?> GetAllKeysAsync(string dbName, int dbVersion, string storeName);

    /// <summary>
    /// Save File without metadata to IndexedDB
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="mimeType"></param>
    /// <returns></returns>
    public Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType);

    /// <summary>
    /// Save File with Metadata to IndexedDB
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="mimeType"></param>
    /// <param name="fileName"></param>
    /// <param name="createdAt"></param>
    /// <param name="lastModifiedAt"></param>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    public Task SaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType,
        string fileName, string createdAt, string lastModifiedAt, string fileFormat);

    /// <summary>
    /// Get File without Metadata from IndexedDB
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<byte[]?> GetFileAsync(string dbName, int dbVersion, string storeName, string key);

    /// <summary>
    /// Get File with Metadata from IndexedDB
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<FileWithMetadata?> GetFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key);

    /// <summary>
    /// Get all file entries with metadata from IndexedDB
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public Task<List<FileWithMetadata>?> GetAllFileEntriesAsync(string dbName, int dbVersion, string storeName);


    /// <summary>
    /// Clear the IndexedDB store content
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="storeName"></param>
    /// <param name="dbVersion"></param>
    /// <returns></returns>
    public Task<bool> ClearStoreAsync(string dbName, string storeName, int dbVersion);

    /// <summary>
    /// Clear all IndexedDB stores for a specific database
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="dbVersion"></param>
    /// <returns></returns>
    public Task<bool> ClearAllStoresAsync(string dbName, int dbVersion);

    /// <summary>
    /// Delete a specific IndexedDB Database
    /// </summary>
    /// <param name="dbName"></param>
    /// <returns></returns>
    public Task<bool> DeleteDatabaseAsync(string dbName);
}