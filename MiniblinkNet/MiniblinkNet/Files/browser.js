
if (window[popHookName]) {
    var popFunc = window[popHookName];

    window.alert = function (msg, opt) {
        return popFunc("alert", msg, opt);
    };
    window.confirm = function (msg, opt) {
        return popFunc("confirm", msg, opt);
    };
    window.prompt = function (msg, value, opt) {
        return popFunc("prompt", msg, value, opt);
    };
}

if (window[openHookName]) {
    var openFunc = window[openHookName];

    window.open = function (url, name, specs, replace) {
        return openFunc(url, name, specs, replace);
    };
}

window.fireDropFileEvent = function (files, x, y, isDone) {
    var e = new CustomEvent("dropFile",
        {
            detail: {
                files: files.split(","),
                x: x,
                y: y,
                isDone: isDone
            }
        });
    window.dispatchEvent(e);
}


