using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Application.Specifications.Yaml;
using Core.Enums;

namespace Application.Services
{
    public class SmartContractService
    {
        private readonly ISolutionPathProvider _solutionPathProvider;
        private readonly IContractModelProvider _contractModelProvider;
        private readonly ISmartContractRepository _contractRepository;
        private readonly ITemplateRepository _templateRepository;
        // private readonly IContractStorage _contractStorage;
        // private readonly IBlockchainService _blockchainService;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public SmartContractService(
            ISolutionPathProvider solutionPathProvider,
            IContractModelProvider contractModelProvider,
            ISmartContractRepository contractRepository,
            ITemplateRepository templateRepository//,
            // IContractStorage contractStorage,
            // IBlockchainService blockchainService)
        )
        {
            _solutionPathProvider = solutionPathProvider;
            _contractModelProvider = contractModelProvider;
            _contractRepository = contractRepository;
            _templateRepository = templateRepository;
            // _contractStorage = contractStorage;
            // _blockchainService = blockchainService;
        }

        public string GetInstancePath(ContractParamsDto paramsDto)
        {
            if (paramsDto.LayoutYaml is null || string.IsNullOrEmpty(paramsDto.LayoutYaml))
            {
                Console.WriteLine("Trying to load model from null or empty yaml, replacing yaml to default {}");

                paramsDto.LayoutYaml = AppConstants.DefaultYamlContent;
            }

            var model = _contractModelProvider.GetContractModelFromYaml(paramsDto.LayoutYaml);
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, ctx, results, validateAllProperties: true))
            {
                var msg = string.Join("; ", results.Select(r => r.ErrorMessage));

                throw new ValidationException($"Invalid contract model: {msg}");
            }

            var solutionDirectory = _solutionPathProvider.GetSolutionRoot();

            return ContractPathHelper.ComputeInstancePath(paramsDto.Area, model, solutionDirectory);
        }

        public async Task<string> LoadYamlTemplate(ContractParamsDto paramsDto)
        {
            return await _templateRepository.LoadAreaModelTemplate(paramsDto.Area);
        }
        
        public async Task<ContractArtefactsDto> GenerateContractCode(ContractParamsDto paramsDto, string instancePath)
        {
            var sem = _locks.GetOrAdd(instancePath, _ => new SemaphoreSlim(1, 1));

            await sem.WaitAsync();

            try
            {
                _contractRepository.CopyBaseHardhatConfigs(instancePath);           // base configs for hardhat launch

                var contractCode = await _templateRepository.GenerateContractCode(paramsDto.Area, paramsDto.LayoutYaml, instancePath);
                var contractTestScript = await _templateRepository.GenerateContractTestScript(paramsDto.Area, paramsDto.LayoutYaml, instancePath);
                var contractTestGasScript = await _templateRepository.GenerateContractTestGasScript(paramsDto.Area, paramsDto.LayoutYaml, instancePath);

                var contractName = _templateRepository.ExtractContractName(paramsDto.LayoutYaml);
                var artefacts = new ContractArtefactsDto
                {
                    InstancePath    = instancePath,
                    Name            = contractName,
                    Code            = contractCode,
                    TestScript      = contractTestScript,
                    TestGasScript   = contractTestGasScript,
                    Status          = ContractStatus.Created,
                    CreatedAt       = DateTime.UtcNow
                };

                // _contractStorage.SaveContractToDb(artefacts);

                return artefacts;
            }
            finally
            {
                sem.Release();
            }
        }

        public string GetContractCode(string instancePath)
        {
            // var artefacts = _contractStorage.GetContractArtefactsFromDb(instancePath);

            // return artefacts.Code;

            throw new NotImplementedException("WANTED");
        }

        public string GetDeployedContractAddress(string instancePath)
        {
            // var artefacts = _contractStorage.GetContractArtefactsFromDb(instancePath);

            // return artefacts.Address;

            throw new NotImplementedException("WANTED");
        }

        public AbiBytecodeDto? GetContractAbiBytecode(string instancePath)
        {
            // var artefacts = _contractStorage.GetContractArtefactsFromDb(instancePath);
            // var contractName = artefacts.Name;

            // return _contractRepository.GetContractAbiBytecode(contractName, instancePath);

            throw new NotImplementedException("WANTED");
        }

        /*
        public async Task<object> GetContractInfoAsync(string contractAddress)
        {
            return await _blockchainService.GetContractInfoAsync(contractAddress);
        }
        */
    }
}
