using System.Net;
using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class HardhatExecutor : IHardhatExecutor
    {
        private readonly ICommandExecutor _commandExecutor;

        public HardhatExecutor(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        public Task<string> DeployContract(string network, string args)
        {
            return _commandExecutor.ExecuteCommandAsync($"npx hardhat run deploy --network {network}", args);
        }

        public Task<string> TestContract(string command, string args)
        {
            return _commandExecutor.ExecuteCommandAsync("npx hardhat test", args);
        }

        public Task<string> VerifyContract(string network, string args)
        {
            const string API_KEY = "Some_api_key"; // replace with one from config or .env file

            return _commandExecutor.ExecuteCommandAsync($"npx hardhat --network {network} etherscan-verify --api-key {API_KEY}", args);
        }
    }
}
