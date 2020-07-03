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
        els[i].addEventListener("click", function() { maxFunc(); });
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
