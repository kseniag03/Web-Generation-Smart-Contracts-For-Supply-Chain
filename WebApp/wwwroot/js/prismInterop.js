// wwwroot/js/prismInterop.js
/*
window.highlightPrism = function () {
    setTimeout(() => {
        Prism.highlightAll();
    }, 50);
};*/

window.highlightPrism = function () {
    setTimeout(() => {
        document.querySelectorAll("pre code").forEach((block) => {
            Prism.highlightElement(block);
        });
    }, 50);
};

window.reRenderCode = (element) => {
    if (element) {
        element.innerHTML = element.innerHTML;
        Prism.highlightAll();
    }
};
