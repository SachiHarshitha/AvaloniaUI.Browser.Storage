using System.Runtime.InteropServices.JavaScript;
using AvaloniaUI.Browser.Storage.Contracts;

using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage
{
    public partial class SessionStorageService : ISessionStorageService
    {
        #region JS Interop

        [JSImport("setItemAsync", "FuncSessionStorage")]
        internal static partial Task JsSetItemAsync(string key, string value);

        [JSImport("getItemAsync", "FuncSessionStorage")]
        internal static partial Task<string?> JsGetItemAsync(string key);

        [JSImport("removeItemAsync", "FuncSessionStorage")]
        internal static partial Task JsRemoveItemAsync(string key);

        [JSImport("clearAsync", "FuncSessionStorage")]
        internal static partial Task JsClearAsync();

        [JSImport("lengthAsync", "FuncSessionStorage")]
        internal static partial Task<double> JsLengthAsync();

        [JSImport("keyAsync", "FuncSessionStorage")]
        internal static partial Task<string?> JsKeyAsync(double index);

        #endregion JS Interop

        #region MyRegion

        public async ValueTask ClearAsync() => await JsClearAsync();

        public async ValueTask<string?> GetItemAsync(string key) => await JsGetItemAsync(key);

        public async ValueTask<string?> KeyAsync(double index) => await JsKeyAsync(index);

        public async ValueTask RemoveItemAsync(string key) => await JsRemoveItemAsync(key);

        public async ValueTask SetItemAsync(string key, string value) => await JsSetItemAsync(key, value);

        public ValueTask<double> Length => new ValueTask<double>(JsLengthAsync());

        #endregion MyRegion
    }
}