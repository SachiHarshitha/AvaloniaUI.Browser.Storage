export async function setItemAsync(key, value) {
    return new Promise((resolve) => {
        sessionStorage.setItem(key, value);
        resolve(true); // Done
    });
}

export async function getItemAsync(key) {
    return new Promise((resolve) => {
        const value = sessionStorage.getItem(key);
        resolve(value);
    });
}

export async function removeItemAsync(key) {
    return new Promise((resolve) => {
        sessionStorage.removeItem(key);
        resolve(true);
    });
}

export async function clearAsync() {
    return new Promise((resolve) => {
        sessionStorage.clear();
        resolve(true);
    });
}

export async function lengthAsync() {
    return new Promise((resolve) => {
        resolve(sessionStorage.length);
    });
}

export async function keyAsync(index) {
    return new Promise((resolve) => {
        resolve(sessionStorage.key(index));
    });
}