import {test, expect} from '@playwright/test';

test.describe('Direct JS-Invokable LocalStorageService tests', () => {
    test.beforeEach(async ({page}) => {
        // Load your WASM app
        await page.goto('http://localhost:5000'); // Change to your actual local dev URL!
    });

    test('should set and get item from localStorage via C# logic', async ({page}) => {
        const key = 'testKey';
        const value = 'testValue';

        // Call C# SetItemInLocalStorage
        await page.evaluate(async ([k, v]) => {
            await DotNet.invokeMethodAsync('YourAssemblyName', 'SetItemInLocalStorage', k, v);
        }, [key, value]);

        // Call C# GetItemFromLocalStorage
        const result = await page.evaluate(async (k) => {
            return await DotNet.invokeMethodAsync('YourAssemblyName', 'GetItemFromLocalStorage', k);
        }, key);

        expect(result).toBe(value);
    });

    test('should remove item from localStorage', async ({page}) => {
        const key = 'testKey';

        // Set item first (via C#)
        await page.evaluate(async ([k, v]) => {
            await DotNet.invokeMethodAsync('YourAssemblyName', 'SetItemInLocalStorage', k, v);
        }, [key, 'someValue']);

        // Remove it
        await page.evaluate(async (k) => {
            await DotNet.invokeMethodAsync('YourAssemblyName', 'RemoveItemFromLocalStorage', k);
        }, key);

        // Verify it's gone
        const value = await page.evaluate((k) => localStorage.getItem(k), key);
        expect(value).toBeNull();
    });

    test('should clear localStorage', async ({page}) => {
        // Add two items (via direct localStorage to set up test)
        await page.evaluate(() => {
            localStorage.setItem('a', '1');
            localStorage.setItem('b', '2');
        });

        // Clear using C# logic
        await page.evaluate(async () => {
            await DotNet.invokeMethodAsync('YourAssemblyName', 'ClearLocalStorage');
        });

        const length = await page.evaluate(() => localStorage.length);
        expect(length).toBe(0);
    });

    test('should get localStorage length and keys via C# logic', async ({page}) => {
        // Setup
        await page.evaluate(() => {
            localStorage.clear();
            localStorage.setItem('key1', 'value1');
            localStorage.setItem('key2', 'value2');
        });

        // Check length
        const length = await page.evaluate(async () => {
            return await DotNet.invokeMethodAsync('YourAssemblyName', 'GetLocalStorageLength');
        });
        expect(length).toBe(2);

        // Check keys
        const key0 = await page.evaluate(async () => {
            return await DotNet.invokeMethodAsync('YourAssemblyName', 'GetKeyAtIndex', 0);
        });
        const key1 = await page.evaluate(async () => {
            return await DotNet.invokeMethodAsync('YourAssemblyName', 'GetKeyAtIndex', 1);
        });

        expect([key0, key1]).toEqual(expect.arrayContaining(['key1', 'key2']));
    });
});
