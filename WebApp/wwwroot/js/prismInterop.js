// wwwroot/js/prismInterop.js

window.highlightAll = function () {
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                const codes = document.querySelectorAll('pre > code');

                codes.forEach(code => {
                    if (code && code.parentNode) {
                        Prism.highlightElement(code);
                    }
                });
            });
        });
    });
};

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
