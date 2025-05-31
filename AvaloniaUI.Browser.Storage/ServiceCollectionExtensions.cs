using AvaloniaUI.Browser.Storage.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaUI.Browser.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBrowserStorage(this IServiceCollection services)
    {
        services.AddSingleton<IIndexedDbFileService, IndexedDbFileService>();
        services.AddSingleton<ILocalStorageService, LocalStorageService>();
        services.AddSingleton<ISessionStorageService, SessionStorageService>();
        return services;
    }
}