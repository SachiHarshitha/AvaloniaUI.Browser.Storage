# AvaloniaWASM.Storage

This library provides a simple way to use browser storage in AvaloniaUI applications. It supports both local, session storage as well as IndexedDB, allowing you to store key-value pairs that persist across sessions or only for the duration of the page load.
The IndexedDB support is useful for storing larger files or structured data.

## Sample Application

| Session / Local Storage    | IndexedDB |
| -------- | ------- |
| ![image](https://github.com/user-attachments/assets/32aa25af-08d2-4a3b-ad2d-8ea231c3e12d)  | ![image](https://github.com/user-attachments/assets/4dcf76ee-dd3d-436b-bf8f-c08f6fe7b25d)    |

## How to Use

1. Install the Nuget Package (Both in the base and startup projects).

2. Add the required JSScript loading to your crossplatform project (File : xxxx.csproj).

```csharp

	# Option  1
	# Direct Reference, Add the following section to your App.cs file or wherever you initialize your application.

	if (OperatingSystem.IsBrowser())
        {
            await JSHost.ImportAsync("FuncLocalStorage", "/js/FuncLocalStorage.js");
            await JSHost.ImportAsync("FuncSessionStorage", "/js/FuncSessionStorage.js");
			await JSHost.ImportAsync("FuncIndexedDbFile", "/js/FuncIndexedDbFile.js");

        }

	..........

	# Option  2
	# For Dependency Injection, inject the services in your `App.cs` file or wherever you configure your services.

	services.AddBrowserStorage();

```

3. Use the `LocalStorage` and `SessionStorage` classes to store and retrieve data in your AvaloniaUI application.
```csharp
	using AvaloniaWASM.Storage;

	.........

	# Initialize the storage services in your class or view model, if not dependecy Injection.
	private SessionStorageService _sessionStorageService = new SessionStorageService();
	private LocalStorageService _localStorageService = new LocalStorageService();

	..........

	# Session Storage Example
	await _sessionStorageService.SetItemAsync("Session_Text", ValueToSetSessionStorage);
	ValueFromSessionStorage = await _sessionStorageService.GetItemAsync("Session_Text");

	...........
	# Local Storage Example
	await _localStorageService.SetItemAsync("Local_Text", ValueToSetLocalStorage);
	ValueFromLocalStorage = await _localStorageService.GetItemAsync("Local_Text");

	...........
	# IndexedDB Example
	await IndexedDbFileService.SaveFileAsync(dbName, storeName, fileName, _fileContent, "text/plain");
	var filefromDB = await IndexedDbFileService.LoadFileAsBase64Async(dbName, storeName, fileName);
```

4. Build startup project to ensure the JavaScript files are copied to the correct location (./wwwroot/js/**).
