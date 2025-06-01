using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaloniaUI.Browser.Storage.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LocalStorageServiceTests : PageTest
{
    private const string ApplicationUrl = "http://localhost:5235";

    [Test]
    public async Task LocalStorage_SetAndGetItem()
    {
        // Load your WASM app instead of playwright.dev
        await Page.GotoAsync(ApplicationUrl); // or wherever your WASM app runs

        // Use JS to call the C# JSInvokable method
        await Page.EvaluateAsync(@"async () => {
            await DotNet.invokeMethodAsync('MyWasmApp', 'SetItemInLocalStorage', 'key1', 'value1');
        }");

        var result = await Page.EvaluateAsync<string>(@"async () => {
            return await DotNet.invokeMethodAsync('MyWasmApp', 'GetItemFromLocalStorage', 'key1');
        }");

        Assert.That(result, Is.EqualTo("value1"));
    }

    [Test]
    public async Task LocalStorage_RemoveItem()
    {
        await Page.GotoAsync(ApplicationUrl);

        // Set, then remove
        await Page.EvaluateAsync(@"async () => {
            await DotNet.invokeMethodAsync('MyWasmApp', 'SetItemInLocalStorage', 'key2', 'toRemove');
        }");

        await Page.EvaluateAsync(@"async () => {
            await DotNet.invokeMethodAsync('MyWasmApp', 'RemoveItemFromLocalStorage', 'key2');
        }");

        // Verify removal via plain JS
        var item = await Page.EvaluateAsync<string>("() => localStorage.getItem('key2')");
        Assert.That(item, Is.Null);
    }

    [Test]
    public async Task LocalStorage_ClearAll()
    {
        await Page.GotoAsync(ApplicationUrl);

        await Page.EvaluateAsync(@"() => {
            localStorage.setItem('a', '1');
            localStorage.setItem('b', '2');
        }");

        await Page.EvaluateAsync(@"async () => {
            await DotNet.invokeMethodAsync('MyWasmApp', 'ClearLocalStorage');
        }");

        var length = await Page.EvaluateAsync<int>("() => localStorage.length");
        Assert.That(length, Is.EqualTo(0));
    }

    [Test]
    public async Task LocalStorage_GetLengthAndKeys()
    {
        await Page.GotoAsync(ApplicationUrl);

        await Page.EvaluateAsync(@"() => {
            localStorage.clear();
            localStorage.setItem('x', '123');
            localStorage.setItem('y', '456');
        }");

        var length = await Page.EvaluateAsync<double>(@"async () => {
            return await DotNet.invokeMethodAsync('MyWasmApp', 'GetLocalStorageLength');
        }");

        Assert.That(length, Is.EqualTo(2));

        var key0 = await Page.EvaluateAsync<string>(@"async () => {
            return await DotNet.invokeMethodAsync('MyWasmApp', 'GetKeyAtIndex', 0);
        }");

        var key1 = await Page.EvaluateAsync<string>(@"async () => {
            return await DotNet.invokeMethodAsync('MyWasmApp', 'GetKeyAtIndex', 1);
        }");

        var keys = new[] { key0, key1 };
        Assert.That(keys, Does.Contain("x").And.Contain("y"));
    }
}