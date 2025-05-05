// wwwroot/js/apiLoginInterop.js

window.apiLogin = async function (url, data) {
    const resp = await fetch(url, {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });

    const text = await resp.text();
    let payload = null;

    try {
        payload = JSON.parse(text);
    } catch {}

    if (!resp.ok) {
        const msg = payload && payload.message
            ? payload.message
            : text || `Error ${resp.status}`;

        throw msg;
    }

    return payload;
};
