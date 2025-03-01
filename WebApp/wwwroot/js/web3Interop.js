// wwwroot/js/web3Interop.js

window.myWeb3Functions = {
    connectMetaMask: function () {
        if (typeof window.ethereum !== "undefined") {
            // Запрос аккаунтов пользователя через MetaMask
            return window.ethereum.request({ method: 'eth_requestAccounts' });
        } else {
            return Promise.reject("MetaMask не обнаружен");
        }
    },
    getSelectedAccount: function () {
        if (window.ethereum && window.ethereum.selectedAddress) {
            return window.ethereum.selectedAddress;
        }
        return null;
    }
};
