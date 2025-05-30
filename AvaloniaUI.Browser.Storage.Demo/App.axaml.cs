using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using AvaloniaUI.Browser.Storage.Demo.ViewModels;
using AvaloniaUI.Browser.Storage.Demo.Views;

using System;
using System.Runtime.InteropServices.JavaScript;
using Avalonia.Controls;

namespace AvaloniaUI.Browser.Storage.Demo;

public partial class App : Application
{
    private static TopLevel _topLevel;

    public static TopLevel TopLevel => _topLevel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (OperatingSystem.IsBrowser())
        {
            await JSHost.ImportAsync("FuncLocalStorage", "/js/FuncLocalStorage.js");
            await JSHost.ImportAsync("FuncSessionStorage", "/js/FuncSessionStorage.js");
            await JSHost.ImportAsync("FuncIndexedDbFile", "/js/FuncIndexedDbFile.js");
        }

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            _topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            _topLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }

        base.OnFrameworkInitializationCompleted();
    }
}