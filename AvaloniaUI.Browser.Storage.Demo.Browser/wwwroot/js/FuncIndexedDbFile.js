export async function openDatabase(dbName, storeName, version = 1) {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, version);

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

export async function saveFileToIndexedDBFromBase64(dbName, dbVersion, storeName, key, base64String, mimeType) {
    const db = await openDatabase(dbName, storeName, dbVersion);
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

export async function saveFileWithMetadata(
    dbName,
    dbVersion,
    storeName,
    key,
    base64String,
    mimeType,
    fileName,
    createdAt,
    lastModifiedAt,
    fileFormat
) {
    const db = await openDatabase(dbName, storeName, dbVersion);

    const binary = atob(base64String);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    const blob = new Blob([bytes], { type: mimeType });

    const fileEntry = {
        key,
        fileName,
        mimeType,
        size: blob.size,
        createdAt,
        lastModifiedAt,
        fileFormat,
        data: blob
    };

    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);
        const req = store.put(fileEntry, key);

        req.onsuccess = () => resolve(true);
        req.onerror = () => reject(req.error);
    });
}

export async function getFileFromIndexedDBAsBase64(dbName, dbVersion, storeName, key) {
    const db = await openDatabase(dbName, storeName, dbVersion);
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
                reader.readAsDataURL(blob.data);
            } else {
                resolve(null);
            }
        };

        getRequest.onerror = () => reject(getRequest.error);
    });
}

export async function getFileWithMetadata(
    dbName,
    dbVersion,
    storeName,
    key
) {
    const db = await openDatabase(dbName, storeName, dbVersion);

    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);

        const getRequest = store.get(key);

        getRequest.onsuccess = () => {
            const fileEntry = getRequest.result;
            if (fileEntry) {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64 = reader.result.split(",")[1];
                    const result = {
                        key: fileEntry.key,
                        fileName: fileEntry.fileName,
                        mimeType: fileEntry.mimeType,
                        size: fileEntry.size,
                        createdAt: fileEntry.createdAt,
                        lastModifiedAt: fileEntry.lastModifiedAt,
                        fileFormat: fileEntry.fileFormat,
                        dataBase64: base64
                    };
                    resolve(result);
                };
                reader.onerror = reject;
                reader.readAsDataURL(fileEntry.data);
            } else {
                resolve(null);
            }
        };

        getRequest.onerror = () => reject(getRequest.error);
    });
}

export async function getAllKeysFromIndexedDB(dbName, dbVersion, storeName) {
    const db = await openDatabase(dbName, storeName, dbVersion);

    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);

        const getAllKeysRequest = store.getAllKeys();

        getAllKeysRequest.onsuccess = () => {
            // Return JSON string
            resolve(JSON.stringify(getAllKeysRequest.result));
        };

        getAllKeysRequest.onerror = () => {
            reject(getAllKeysRequest.error);
        };
    });
}

export async function getAllFileEntriesFromIndexedDB(dbName, dbVersion, storeName) {
    const db = await openDatabase(dbName, storeName, dbVersion);

    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);
        const req = store.getAll();

        req.onsuccess = () => {
            const entries = req.result.map(entry => {
                const {data, ...rest} = entry;
                return rest;
            });
            resolve(JSON.stringify(entries));
        };

        req.onerror = () => reject(req.error);
    });
}