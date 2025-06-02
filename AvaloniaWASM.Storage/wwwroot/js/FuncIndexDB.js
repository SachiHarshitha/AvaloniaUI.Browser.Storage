export async function openDb(dbName, storeName) {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, 1);

        request.onerror = (event) => {
            reject(event.target.error);
        };

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName, { keyPath: "id" });
            }
        };

        request.onsuccess = (event) => {
            resolve(event.target.result);
        };
    });
}

export async function addItemAsync(dbName, storeName, item) {
    const db = await openDb(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);
        const request = store.put(item);

        request.onsuccess = () => resolve();
        request.onerror = (event) => reject(event.target.error);
    });
}

export async function getItemAsync(dbName, storeName, id) {
    const db = await openDb(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);
        const request = store.get(id);

        request.onsuccess = () => resolve(request.result);
        request.onerror = (event) => reject(event.target.error);
    });
}

export async function deleteItemAsync(dbName, storeName, id) {
    const db = await openDb(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);
        const request = store.delete(id);

        request.onsuccess = () => resolve();
        request.onerror = (event) => reject(event.target.error);
    });
}

export async function clearAllAsync(dbName, storeName) {
    const db = await openDb(dbName, storeName);
    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);
        const request = store.clear();

        request.onsuccess = () => resolve();
        request.onerror = (event) => reject(event.target.error);
    });
}