using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using AvaloniaUI.Browser.Storage.Contracts;

namespace AvaloniaUI.Browser.Storage;

public partial class LocalStorageService : ILocalStorageService
{
    #region JS Interop

    [JSImport("setItemAsync", "FuncLocalStorage")]
    internal static partial Task JsSetItemAsync(string key, string value);

    [JSImport("getItemAsync", "FuncLocalStorage")]
    internal static partial Task<string?> JsGetItemAsync(string key);

    [JSImport("removeItemAsync", "FuncLocalStorage")]
    internal static partial Task JsRemoveItemAsync(string key);

    [JSImport("clearAsync", "FuncLocalStorage")]
    internal static partial Task JsClearAsync();

    [JSImport("lengthAsync", "FuncLocalStorage")]
    internal static partial Task<double> JsLengthAsync();

    [JSImport("keyAsync", "FuncLocalStorage")]
    internal static partial Task<string?> JsKeyAsync(double index);

    #endregion JS Interop

    #region Public Methods

    // Public API implementation for ILocalStorageService
    public async ValueTask ClearAsync() => await JsClearAsync();

    public async ValueTask<string?> GetItemAsync(string key) => await JsGetItemAsync(key);

    public async ValueTask<string?> KeyAsync(double index) => await JsKeyAsync(index);

    public async ValueTask RemoveItemAsync(string key) => await JsRemoveItemAsync(key);

    public async ValueTask SetItemAsync(string key, string value) => await JsSetItemAsync(key, value);

    public ValueTask<double> Length => new ValueTask<double>(JsLengthAsync());

    #endregion Public Methods
}