// wwwroot/js/web3Interop.js

window.myWeb3Functions = {
    connectMetaMask: async function () {
        if (typeof window.ethereum !== "undefined") {
            return await window.ethereum.request({ method: 'eth_requestAccounts' });
        } else {
            return Promise.reject("MetaMask is not installed");
        }
    },
    getSelectedAccount: function () {
        if (window.ethereum && window.ethereum.selectedAddress) {
            return window.ethereum.selectedAddress;
        }
        return null;
    }
};
