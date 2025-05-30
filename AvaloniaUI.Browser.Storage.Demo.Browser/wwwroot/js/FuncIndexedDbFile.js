export async function openDatabase(dbName, storeName) {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, 1);

        request.onupgradeneeded = function (event) {
            const db = event.target.result;
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName);
            }
        };

        request.onsuccess = function (event) {
            resolve(event.target.result);
        };

        request.onerror = function (event) {
            reject(event.target.error);
        };
    });
}

export async function saveFileToIndexedDBFromBase64(dbName, storeName, key, base64String, mimeType) {
    const db = await openDatabase(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);

        // Convert base64 string to ArrayBuffer
        const binaryString = atob(base64String);
        const len = binaryString.length;
        const bytes = new Uint8Array(len);
        for (let i = 0; i < len; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }

        const blob = new Blob([bytes], { type: mimeType });

        const putRequest = store.put(blob, key);

        putRequest.onsuccess = () => resolve(true);
        putRequest.onerror = () => reject(putRequest.error);
    });
}

export async function getFileFromIndexedDBAsBase64(dbName, storeName, key) {
    const db = await openDatabase(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);

        const getRequest = store.get(key);

        getRequest.onsuccess = () => {
            const blob = getRequest.result;
            if (blob) {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64 = reader.result.split(',')[1];
                    resolve(base64);
                };
                reader.onerror = reject;
                reader.readAsDataURL(blob);
            } else {
                resolve(null);
            }
        };

        getRequest.onerror = () => reject(getRequest.error);
    });
}