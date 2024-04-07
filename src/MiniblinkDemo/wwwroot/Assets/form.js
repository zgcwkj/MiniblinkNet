//窗口状态改变
window.maxsizechanged = function (state) {
    var el = $('.ui-sys-commands > span:eq(1)');
    if (state == 2) {
        el.text('\u0031');
    }
    else {
        el.text('\u0032');
    }
};
window.minsizechanged = function (state) {
};
//窗口激活状态改变
window.activated = function () {
    $(".ui-nav").removeClass("ui-nav-color");
};
window.deactivate = function () {
    $(".ui-nav").addClass("ui-nav-color");
};
//窗口尺寸改变
window.sizechanged = function (w, h) {
    console.log("sizechanged", w, h)
};
