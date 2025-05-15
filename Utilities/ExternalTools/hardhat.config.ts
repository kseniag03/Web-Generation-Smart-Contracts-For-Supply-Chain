import { HardhatUserConfig } from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";

const config: HardhatUserConfig = {
  solidity: "0.8.22",
  networks: {
    binance_testnet: {
      url: `https://bsc-prebsc-dataseed.bnbchain.org/`,
      chainId: 97,
      accounts: [] // No deploy from backend
    },
    hardhat: {}
  },
  namedAccounts: {
    deployer: 0,
  },
  paths: {
    sources: "./contract",
    tests: "./test",
    artifacts: "./artifacts",
    cache: "./cache"
  }
};

export default config;