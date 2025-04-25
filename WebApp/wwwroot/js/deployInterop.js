// wwwroot/js/deployInterop.js

window.deployFunctions = {
    deployWithMetaMask: async function (abi, bytecode, constructorArgs) {
        try {
            if (!window.ethereum) {
                throw new Error("MetaMask is not installed");
            }

            const provider = new ethers.BrowserProvider(window.ethereum);
            const signer = await provider.getSigner();

            const factory = new ethers.ContractFactory(abi, bytecode, signer);
            const contract = await factory.deploy(...constructorArgs);
            await contract.waitForDeployment();

            return await contract.getAddress();
        } catch (error) {
            return "ERROR: " + error.message;
        }
    }
};
