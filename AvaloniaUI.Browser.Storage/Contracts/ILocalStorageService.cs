using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Contracts
{
    /// <summary>
    /// Reference: https://github.com/IEvangelist/blazorators/tree/main/src/Blazor.LocalStorage
    /// </summary>
    public interface ILocalStorageService
    {
        /// <summary>
        /// Source generated implementation of <c>window.localStorage.clear</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/clear"></a>
        /// </summary>
        ValueTask ClearAsync();

        /// <summary>
        /// Source generated implementation of <c>window.localStorage.getItem</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/getItem"></a>
        /// </summary>
        ValueTask<string?> GetItemAsync(string key);

        /// <summary>
        /// Source generated implementation of <c>window.localStorage.key</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/key"></a>
        /// </summary>
        ValueTask<string?> KeyAsync(double index);

        /// <summary>
        /// Source generated implementation of <c>window.localStorage.removeItem</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/removeItem"></a>
        /// </summary>
        ValueTask RemoveItemAsync(string key);

        /// <summary>
        /// Source generated implementation of <c>window.localStorage.setItem</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/setItem"></a>
        /// </summary>
        ValueTask SetItemAsync(string key, string value);

        /// <summary>
        /// Source generated implementation of <c>window.localStorage.length</c>.
        /// <a href="https://developer.mozilla.org/docs/Web/API/Storage/length"></a>
        /// </summary>
        ValueTask<double> Length { get; }
    }
}