using Microsoft.JSInterop;

namespace AvaloniaUI.Browser.Storage.Tests;

public static partial class LocalStorageTestInterop
{
    [JSInvokable("SetItemInLocalStorage")]
    public static async Task SetItem(string key, string value)
    {
        var service = new LocalStorageService();
        await service.SetItemAsync(key, value);
    }

    [JSInvokable("GetItemFromLocalStorage")]
    public static async Task<string?> GetItem(string key)
    {
        var service = new LocalStorageService();
        return await service.GetItemAsync(key);
    }

    [JSInvokable("RemoveItemFromLocalStorage")]
    public static async Task RemoveItem(string key)
    {
        var service = new LocalStorageService();
        await service.RemoveItemAsync(key);
    }

    [JSInvokable("ClearLocalStorage")]
    public static async Task Clear()
    {
        var service = new LocalStorageService();
        await service.ClearAsync();
    }

    [JSInvokable("GetLocalStorageLength")]
    public static async Task<double> GetLength()
    {
        var service = new LocalStorageService();
        return await service.GetLength();
    }

    [JSInvokable("GetKeyAtIndex")]
    public static async Task<string?> GetKey(double index)
    {
        var service = new LocalStorageService();
        return await service.KeyAsync(index);
    }
}