// wwwroot/js/web3Interop.js

window.myWeb3Functions = {

    connectMetaMask: async function () {
        if (typeof window.ethereum === "undefined") {
            // ������� ������, � �� ������ - ��������� ����� � C#
            throw "MetaMask is not installed or disabled";
        }

        try {
            const accounts = await window.ethereum.request({
                method: "eth_requestAccounts"
            });

            return accounts;          // string[]
        } catch (err) {
            // err ����� ���� �������� { code, message }
            throw err.message ?? err;
        }
    },

    getSelectedAccount: function () {
        if (window.ethereum && window.ethereum.selectedAddress) {
            return window.ethereum.selectedAddress;
        }
        return null;
    }
};
