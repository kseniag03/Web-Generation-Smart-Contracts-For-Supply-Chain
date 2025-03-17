// using Core.Entities;
using Application.DTOs;
using Core.Interfaces;

namespace Application.Services
{
    public class SmartContractService
    {
        private readonly ISmartContractRepository _contractRepository;
        // private readonly IBlockchainService _blockchainService;

        public SmartContractService(ISmartContractRepository contractRepository)//, IBlockchainService blockchainService)
        {
            _contractRepository = contractRepository;
            // _blockchainService = blockchainService;
        }

        public string GenerateContractCode(ContractParamsDto paramsDto)
        {
            return _contractRepository.GenerateContractCode(
                paramsDto.ContractName,
                paramsDto.ApplicationArea,
                paramsDto.UintType,
                paramsDto.EnableEvents,
                paramsDto.IncludeVoidLabel);
        }

        public string GetContractCode(string contractName)
        {
            return _contractRepository.GetContractCode(contractName);
        }
        /*
        public async Task<string> DeployContractAsync(string contractName)
        {
            string contractCode = _contractRepository.GetContractCode(contractName);
            if (string.IsNullOrEmpty(contractCode)) return null;

            return await _blockchainService.DeployContractAsync(contractCode);
        }
        */
        public string GetDeployedContractAddress(string contractName)
        {
            return _contractRepository.GetDeployedContractAddress(contractName);
        }
        /*
        public async Task<bool> TestContractAsync(string contractName)
        {
            return await _blockchainService.TestContractAsync(contractName);
        }

        public async Task<object> GetContractInfoAsync(string contractAddress)
        {
            return await _blockchainService.GetContractInfoAsync(contractAddress);
        }
        */
    }
}
