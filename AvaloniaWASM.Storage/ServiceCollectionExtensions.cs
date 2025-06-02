using AvaloniaWASM.Storage.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaWASM.Storage;

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