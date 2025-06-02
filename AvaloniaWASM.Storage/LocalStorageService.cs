using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using AvaloniaWASM.Storage.Contracts;

namespace AvaloniaWASM.Storage;

public partial class LocalStorageService : ILocalStorageService
{
    #region Fields

    private const string ModuleName = "FuncLocalStorage";

    private const string ModulePath = "/js/FuncLocalStorage.js";

    private JSObject? _indexedDbModule;

    #endregion Fields

    #region JS Interop

    [JSImport("setItemAsync", ModuleName)]
    internal static partial Task JsSetItemAsync(string key, string value);

    [JSImport("getItemAsync", ModuleName)]
    internal static partial Task<string?> JsGetItemAsync(string key);

    [JSImport("removeItemAsync", ModuleName)]
    internal static partial Task JsRemoveItemAsync(string key);

    [JSImport("clearAsync", ModuleName)]
    internal static partial Task JsClearAsync();

    [JSImport("lengthAsync", ModuleName)]
    internal static partial Task<double> JsLengthAsync();

    [JSImport("keyAsync", ModuleName)]
    internal static partial Task<string?> JsKeyAsync(double index);

    #endregion JS Interop

    public LocalStorageService()
    {
        _ = InitializeAsync();
    }

    #region Methods

    /// <summary>
    /// Initialization method
    /// </summary>
    /// <returns></returns>
    private async Task InitializeAsync()
    {
        _indexedDbModule = await JSHost.ImportAsync(ModuleName, ModulePath);
    }

    public async ValueTask ClearAsync()
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        await JsClearAsync();
    }

    public async ValueTask<string?> GetItemAsync(string key)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsGetItemAsync(key);
    }

    public async ValueTask<string?> KeyAsync(double index)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsKeyAsync(index);
    }

    public async ValueTask RemoveItemAsync(string key)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        await JsRemoveItemAsync(key);
    }

    public async ValueTask SetItemAsync(string key, string value)
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        await JsSetItemAsync(key, value);
    }

    public async ValueTask<double> GetLength()
    {
        _indexedDbModule ??= await JSHost.ImportAsync(ModuleName, ModulePath);
        return await JsLengthAsync();
    }

    #endregion Methods
}