export async function setItemAsync(key, value) {
    return new Promise((resolve) => {
        localStorage.setItem(key, value);
        resolve();
    });
}

export async function getItemAsync(key) {
    return new Promise((resolve) => {
        const value = localStorage.getItem(key);
        resolve(value);
    });
}

export async function removeItemAsync(key) {
    return new Promise((resolve) => {
        localStorage.removeItem(key);
        resolve();
    });
}

export async function clearAsync() {
    return new Promise((resolve) => {
        localStorage.clear();
        resolve();
    });
}

export async function lengthAsync() {
    return new Promise((resolve) => {
        resolve(localStorage.length);
    });
}

export async function keyAsync(index) {
    return new Promise((resolve) => {
        resolve(localStorage.key(index));
    });
}