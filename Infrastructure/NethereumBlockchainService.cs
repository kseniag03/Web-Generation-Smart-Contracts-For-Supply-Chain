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
    }

}
