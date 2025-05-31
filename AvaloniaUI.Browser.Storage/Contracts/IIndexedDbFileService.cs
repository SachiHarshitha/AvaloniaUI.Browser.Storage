using System.Threading.Tasks;

using AvaloniaUI.Browser.Storage.Models;

namespace AvaloniaUI.Browser.Storage.Contracts;

public interface IIndexedDbFileService
{
    public Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType);

    public Task SaveFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType,
        string fileName, string createdAt, string lastModifiedAt, string fileFormat);

    public Task<byte[]?> GetFileAsync(string dbName, int dbVersion, string storeName, string key);

    public Task<FileWithMetadata?> GetFileWithMetadataAsync(string dbName, int dbVersion, string storeName, string key);
}