// wwwroot/js/apiLoginInterop.js

window.apiLogin = async function (url, data) {
    const resp = await fetch(url, {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    if (!resp.ok) {
        const text = await resp.text();
        throw new Error(text || resp.status);
    }
    const result = await resp.json();
    return result;
};
