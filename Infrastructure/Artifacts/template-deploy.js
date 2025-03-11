import hardhat from "hardhat";

const { ethers } = hardhat;

async function main() {
    const [deployer] = await ethers.getSigners();

    console.log("Deploying IOTContractMonitoring with account:", deployer.address);

    const IOTContractMonitoring = await ethers.getContractFactory("IOTContractMonitoring", deployer);
    const token = await IOTContractMonitoring.deploy(deployer.address);

    await token.deployed();

    console.log("IOTContractMonitoring deployed to:", token.address);
}

main()
    .then(() => {
        console.log("Deployment and interaction completed");
        process.exit(0);
    })
    .catch((error) => {
        console.error("Deployment failed:", error);
        process.exit(1);
    });
