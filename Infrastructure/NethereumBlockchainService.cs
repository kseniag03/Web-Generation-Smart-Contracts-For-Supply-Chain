using Core.Interfaces;
using Nethereum.Web3;

namespace Infrastructure.Blockchain
{
    public class NethereumBlockchainService : IBlockchainService
    {
        private readonly Web3 _web3;

        public NethereumBlockchainService(string rpcUrl)
        {
            _web3 = new Web3(rpcUrl);
        }

        public async Task<string> DeployContractAsync(string contractCode)
        {
            var senderAddress = "0xYourAddress";
            return await _web3.Eth.DeployContract.SendRequestAsync(senderAddress, contractCode);
        }

        public async Task<bool> TestContractAsync(string contractName)
        {
            return true; // testing logic
        }

        public async Task<object> GetContractInfoAsync(string contractAddress)
        {
            return new { Address = contractAddress, Status = "Active" };
        }
    }
}


/*
using Core.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Infrastructure
{
    public class NethereumBlockchainService : IBlockchainService
    {
        private readonly Web3 _web3;
        private readonly string _senderAddress; // Адрес отправителя

        public NethereumBlockchainService(string rpcUrl, string senderAddress)
        {
            _web3 = new Web3(rpcUrl);
            _senderAddress = senderAddress;
        }

        public async Task<string> DeployContractAsync(string contractBytecode)
        {
            var gas = new HexBigInteger(3000000); // Указать лимит газа
            var transactionInput = new TransactionInput(contractBytecode, _senderAddress)
            {
                Gas = gas
            };

            // Используем корректный метод
            var txHash = await _web3.Eth.DeployContract.SendRequestAsync(transactionInput.From, transactionInput.Data, transactionInput.Gas);

            return txHash;
        }

        public Task<object> GetContractInfoAsync(string contractAddress)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendTransactionAsync(string to, decimal amount)
        {
            var valueInWei = Web3.Convert.ToWei(amount); // Перевод ETH в Wei
            var gasLimit = new HexBigInteger(21000); // Лимит газа для простой транзакции

            var transactionInput = new TransactionInput
            {
                From = _senderAddress,
                To = to,
                Value = new HexBigInteger(valueInWei),
                Gas = gasLimit
            };

            var txHash = await _web3.Eth.Transactions.SendTransaction.SendRequestAsync(transactionInput);

            return txHash;
        }

        public Task<bool> TestContractAsync(string contractName)
        {
            throw new NotImplementedException();
        }
    }

}
*/
