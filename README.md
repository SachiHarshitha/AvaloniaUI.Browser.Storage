# AvaloniaUI.Browser.Storage

This library provides a simple way to use browser storage in AvaloniaUI applications. It supports both local and session storage, allowing you to store key-value pairs that persist across sessions or only for the duration of the page load.

## Sample Application

| Session / Local Storage    | IndexedDB |
| -------- | ------- |
| ![image](https://github.com/user-attachments/assets/32aa25af-08d2-4a3b-ad2d-8ea231c3e12d)  | ![image](https://github.com/user-attachments/assets/4dcf76ee-dd3d-436b-bf8f-c08f6fe7b25d)    |



## How to Use

Currently , the library is in the early stages of development. To use it, you can clone the repository and reference the project in your AvaloniaUI application.

1. Clone the repository:
```bash
   git clone https://github.com/SachiHarshitha/AvaloniaUI.Browser.Storage.git
 ```

2. Add a reference to the `AvaloniaUI.Browser.Storage` project in your AvaloniaUI application (Both in the base and startup projects).

3. Add the following section to the Browser startup project (File : xxxx.Browser.csproj).

```xml
<!--Section :Direct Reference, Add following section if you refer the project directly-->
	<ItemGroup>
		<JsFiles Include="..\AvaloniaUI.Browser.Storage\wwwroot\js\**\*.*" />
	</ItemGroup>

	<Target Name="CopyJsFilesBeforeBuild" BeforeTargets="Build">
		<Message Importance="high" Text="Copying JS files to wwwroot..." />
		<Copy
			SourceFiles="@(JsFiles)"
			DestinationFolder="$(MSBuildProjectDirectory)\wwwroot\js\%(RecursiveDir)"
			SkipUnchangedFiles="true" />
	</Target>
<!-- End Section-->
```

4. Add the required JSScript loading to your crossplatform project (File : xxxx.csproj).

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

5. Use the `LocalStorage` and `SessionStorage` classes to store and retrieve data in your AvaloniaUI application.
```csharp
	using AvaloniaUI.Browser.Storage;

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