using Application.DTOs;
using Application.Interfaces;
using Application.Mappings;

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

        public string GenerateContractCode(ContractParamsDto paramsDto, string instancePath)
        {
            return string.Empty; // _contractRepository.GenerateContractCode(paramsDto.Area, instancePath);
        }

        public string GetContractCode(string contractName, string instancePath)
        {
            return _contractRepository.GetContractCode(contractName, instancePath);
        }

        public string GetDeployedContractAddress(string contractName, string instancePath)
        {
            return _contractRepository.GetDeployedContractAddress(contractName, instancePath);
        }

        public AbiBytecodeDto? GetContractAbiBytecode(string contractName, string instancePath)
        {
            return _contractRepository.GetContractAbiBytecode(contractName, instancePath)?.ToDto();
        }

        /*
        public async Task<object> GetContractInfoAsync(string contractAddress)
        {
            return await _blockchainService.GetContractInfoAsync(contractAddress);
        }
        */
    }
}
