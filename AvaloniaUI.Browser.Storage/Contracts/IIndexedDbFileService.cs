using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Contracts;

public interface IIndexedDbFileService
{
    public Task SaveFileAsync(string dbName, int dbVersion, string storeName, string key, byte[] data,
        string mimeType);

    public Task<byte[]?> GetFileAsync(string dbName, int dbVersion, string storeName, string key);
}