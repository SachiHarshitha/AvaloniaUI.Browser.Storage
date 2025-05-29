using AvaloniaUI.Browser.Storage.Contracts;

using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage;

public static class StorageExtensions
{
    public static async ValueTask SetItemAsJsonAsync<T>(
        this ILocalStorageService storage, string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await storage.SetItemAsync(key, json);
    }

    public static async ValueTask<T?> GetItemFromJsonAsync<T>(
        this ILocalStorageService storage, string key)
    {
        var json = await storage.GetItemAsync(key);
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public static async ValueTask SetItemAsJsonAsync<T>(
        this ISessionStorageService storage, string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await storage.SetItemAsync(key, json);
    }

    public static async ValueTask<T?> GetItemFromJsonAsync<T>(
        this ISessionStorageService storage, string key)
    {
        var json = await storage.GetItemAsync(key);
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }
}