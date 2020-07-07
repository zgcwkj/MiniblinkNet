// 拖动和禁用拖动元素
if (window[dragName]) {
    var dragFunc = window[dragName];
    var els = document.getElementsByTagName("html");
    for (var i = 0; i < els.length; i++) {
        els[i].onmousedown = function (e) {
            var obj = e.target || e.srcElement;
            if ({ "INPUT": 1, "SELECT": 1 }[obj.tagName.toUpperCase()])
                return;
            // 锁定
            while (obj) {
                for (var i = 0; i < obj.classList.length; i++) {
                    if (obj.classList[i] === "mbform-nodrag")
                        return;
                    if (obj.classList[i] === "mbform-drag") {
                        dragFunc();
                        return;
                    }
                }
                obj = obj.parentElement;
            }
        }
    }
}
// 双击最大化元素
if (window[maxName]) {
    var maxFunc = window[maxName];
    var els = document.getElementsByClassName("mbform-max-double");
    for (var i = 0; i < els.length; i++) {
        els[i].ondblclick = function () { maxFunc(); }
    }
}
// 最大化元素
if (window[maxName]) {
    var maxFunc = window[maxName];
    var els = document.getElementsByClassName("mbform-max");
    for (var i = 0; i < els.length; i++) {
        els[i].addEventListener("click", function () { maxFunc(); });
    }
}
// 最小化元素
if (window[minName]) {
    var minFunc = window[minName];
    var els = document.getElementsByClassName("mbform-min");
    for (var i = 0; i < els.length; i++) {
        els[i].addEventListener("click", function () { minFunc(); });
    }
}
// 关闭元素
if (window[closeName]) {
    var closeFunc = window[closeName];
    var els = document.getElementsByClassName("mbform-close");
    for (var i = 0; i < els.length; i++) {
        els[i].addEventListener("click", function () { closeFunc(); });
    }
}
//网络请求
window.ajax = {
    get: function (url, fn) {
        // XMLHttpRequest 对象用于在后台与服务器交换数据  
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.onreadystatechange = function () {
            // readyState == 4说明请求已完成
            if (xhr.readyState == 4 && xhr.status == 200 || xhr.status == 304) {
                // 从服务器获得数据 
                fn.call(this, xhr.responseText);
            }
        };
        xhr.send();
    },
    post: function (url, data, fn) {
        // XMLHttpRequest 对象用于在后台与服务器交换数据  
        var xhr = new XMLHttpRequest();
        xhr.open("POST", url, true);
        // 添加http头，发送信息至服务器时内容编码类型
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 304)) {
                fn.call(this, xhr.responseText);
            }
        };
        xhr.send(data);
    }
}
