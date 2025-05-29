using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaUI.Browser.Storage.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private SessionStorageService _sessionStorageService = new SessionStorageService();
    private LocalStorageService _localStorageService = new LocalStorageService();
    public string Greeting => "Welcome to Avalonia!";

    [RelayCommand]
    public async Task SessionStorageCommand()
    {
        await _sessionStorageService.SetItemAsync("Session_Text", ValueToSetSessionStorage);
        ValueFromSessionStorage = await _sessionStorageService.GetItemAsync("Session_Text");
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task LocalStorageCommand()
    {
        // This is a placeholder for the local storage command logic.
        // You can implement your local storage logic here.
        await _localStorageService.SetItemAsync("Local_Text", ValueToSetLocalStorage);
        ValueFromLocalStorage = await _localStorageService.GetItemAsync("Local_Text");
        await Task.CompletedTask;
    }

    [ObservableProperty]
    private string _valueToSetLocalStorage = string.Empty;

    [ObservableProperty]
    private string _valueToSetSessionStorage = string.Empty;

    [ObservableProperty]
    private string _valueFromSessionStorage = string.Empty;

    [ObservableProperty]
    private string _valueFromLocalStorage = string.Empty;
}