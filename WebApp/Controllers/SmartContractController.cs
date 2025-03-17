using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs;
// using Core.Entities;

namespace WebApp.Controllers
{
    // https://{domain}/api/contracts/{action}
    [ApiController]
    [Route("api/contracts")]
    public class SmartContractController : ControllerBase
    {
        private readonly SmartContractService _contractService;

        public SmartContractController(SmartContractService contractService)
        {
            _contractService = contractService;
        }

        /// <summary>
        /// Генерирует код смарт-контракта на основе шаблона
        /// </summary>
        [HttpPost("generate")]
        public IActionResult GenerateContract(ContractParamsDto paramsDto)
        {
            string contractCode = _contractService.GenerateContractCode(paramsDto);
            return Ok(new { contractName = paramsDto.ContractName, code = contractCode });
        }

        /// <summary>
        /// Получает сгенерированный код смарт-контракта
        /// </summary>
        [HttpGet("{contractName}/code")]
        public IActionResult GetContractCode(string contractName)
        {
            string contractCode = _contractService.GetContractCode(contractName);
            if (string.IsNullOrEmpty(contractCode))
                return NotFound("Contract code not found.");

            return Ok(new { contractName, code = contractCode });
        }
        /*
        /// <summary>
        /// Деплоит контракт в блокчейн и возвращает его публичный адрес
        /// </summary>
        [HttpPost("{contractName}/deploy")]
        public async Task<IActionResult> DeployContract(string contractName)
        {
            string deploymentAddress = await _contractService.DeployContractAsync(contractName);
            if (string.IsNullOrEmpty(deploymentAddress))
                return BadRequest("Failed to deploy contract.");

            return Ok(new { contractName, address = deploymentAddress });
        }
        */
        /// <summary>
        /// Получает публичный адрес задеплоенного контракта
        /// </summary>
        [HttpGet("{contractName}/address")]
        public IActionResult GetDeployedContractAddress(string contractName)
        {
            string contractAddress = _contractService.GetDeployedContractAddress(contractName);
            if (string.IsNullOrEmpty(contractAddress))
                return NotFound("Contract is not deployed.");

            return Ok(new { contractName, address = contractAddress });
        }
        /*
        /// <summary>
        /// Тестирует контракт, выполняя тестовые вызовы
        /// </summary>
        [HttpPost("{contractName}/test")]
        public async Task<IActionResult> TestContract(string contractName)
        {
            bool success = await _contractService.TestContractAsync(contractName);
            if (!success)
                return BadRequest("Contract test failed.");

            return Ok($"Contract {contractName} passed all tests.");
        }

        /// <summary>
        /// Получает информацию о контракте из блокчейна
        /// </summary>
        [HttpGet("{contractAddress}/info")]
        public async Task<IActionResult> GetContractInfo(string contractAddress)
        {
            var contractInfo = await _contractService.GetContractInfoAsync(contractAddress);
            if (contractInfo == null)
                return NotFound("Contract info not found.");

            return Ok(contractInfo);
        }
        */

        [Authorize(Roles = "Auditor, Deployer, Admin")]
        [HttpPost("audit")]
        public async Task<IActionResult> AuditContract()
        {
            return Ok("Running security audit...");
        }

        [Authorize(Roles = "Deployer, Admin")]
        [HttpPost("deploy")]
        public async Task<IActionResult> DeployContract()
        {
            return Ok("Deploying contract...");
        }

    }
}
