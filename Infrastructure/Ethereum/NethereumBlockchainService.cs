namespace Infrastructure.Ethereum
{/*
    public class NethereumBlockchainService : IBlockchainService
    {
        private readonly Web3 _web3;
        private readonly string _senderAddress;

        public NethereumBlockchainService(string rpcUrl, string senderAddress)
        {
            _web3 = new Web3(rpcUrl);
            _senderAddress = senderAddress;
        }

        public async Task<string> DeployContractAsync(string contractBytecode)
        {
            var gas = new HexBigInteger(3000000); // gas limit
            var transactionInput = new TransactionInput(contractBytecode, _senderAddress)
            {
                Gas = gas
            };

            var txHash = await _web3.Eth.DeployContract.SendRequestAsync(transactionInput.From, transactionInput.Data, transactionInput.Gas);

            return txHash;
        }

        public async Task<object> GetContractInfoAsync(string contractAddress)
        {
            return new { Address = contractAddress, Status = "Active" };
        }
    }*/
}
