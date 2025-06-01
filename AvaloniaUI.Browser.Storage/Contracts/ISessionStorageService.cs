using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Contracts
{
    /// <summary>
    /// Reference: https://github.com/IEvangelist/blazorators/tree/main/src/Blazor.SessionStorage
    /// </summary>
    public interface ISessionStorageService
    {
        /// <summary>
        /// Clear all items in session storage.
        /// </summary>
        /// <returns></returns>
        ValueTask ClearAsync();

        /// <summary>
        /// Get Item Method
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ValueTask<string?> GetItemAsync(string key);

        /// <summary>
        /// Get Item Key by Index Method
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ValueTask<string?> KeyAsync(double index);

        /// <summary>
        /// Remove Item Method
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ValueTask RemoveItemAsync(string key);

        /// <summary>
        /// Set Item Value Method
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ValueTask SetItemAsync(string key, string value);

        /// <summary>
        /// Get Length Method
        /// </summary>
        /// <returns></returns>
        ValueTask<double> GetLength();
    }
}