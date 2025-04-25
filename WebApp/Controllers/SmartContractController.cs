using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs;
using Utilities.Interfaces;
using Infrastructure.Repositories.Helpers;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class SmartContractController : ControllerBase
    {
        private readonly SmartContractService _contractService;

        private readonly IHardhatExecutor _hh;
        private readonly IFoundryExecutor _fh;
        private readonly ISlitherExecutor _sh;

        public SmartContractController(
            SmartContractService contractService,
            IHardhatExecutor hh,
            IFoundryExecutor fh,
            ISlitherExecutor sh)
        {
            _contractService = contractService;

            _hh = hh;
            _fh = fh;
            _sh = sh;
        }

        /// <summary>
        /// Генерирует код смарт-контракта на основе шаблона
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContractAsync(ContractParamsDto paramsDto)
        {
            if (paramsDto == null)
            {
                return BadRequest("Missing contract parameters.");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);
            var contractCode = _contractService.GenerateContractCode(paramsDto, instancePath);

            if (_hh is not null && !string.IsNullOrEmpty(contractCode))
            {
                var compileResult = await _hh.CompileContract(instancePath);
                // logging
            }

            return Ok(new
            {
                contractName = paramsDto.ContractName,
                code = contractCode,
                instancePath
            });
        }

        /// <summary>
        /// Получает сгенерированный код смарт-контракта
        /// </summary>
        [HttpGet("code/{contractName}")]
        public IActionResult GetContractCode(ContractParamsDto paramsDto)
        {
            if (paramsDto == null || string.IsNullOrEmpty(paramsDto.ContractName))
            {
                return BadRequest("Missing contract name parameter.");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);
            var contractCode = _contractService.GetContractCode(paramsDto.ContractName, instancePath);

            if (string.IsNullOrEmpty(contractCode))
            {
                return NotFound("Contract code not found.");
            }

            return Ok(new { paramsDto.ContractName, code = contractCode });
        }

        /// <summary>
        /// Получает публичный адрес задеплоенного контракта
        /// </summary>
        [HttpGet("address/{contractName}")]
        public IActionResult GetDeployedContractAddress(ContractParamsDto paramsDto)
        {
            if (paramsDto == null || string.IsNullOrEmpty(paramsDto.ContractName))
            {
                return BadRequest("Missing contract name parameter.");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);
            var contractAddress = _contractService.GetDeployedContractAddress(paramsDto.ContractName, instancePath);

            if (string.IsNullOrEmpty(contractAddress))
            {
                return NotFound("Contract is not deployed.");
            }

            return Ok(new { paramsDto.ContractName, address = contractAddress });
        }

        [HttpGet("abi-bytecode/{contractName}")]
        public IActionResult GetAbiAndBytecode(ContractParamsDto paramsDto)
        {
            if (paramsDto == null || string.IsNullOrEmpty(paramsDto.ContractName))
            {
                return BadRequest("Missing contract name parameter.");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);
            var abiBytecodeDto = _contractService.GetContractAbiBytecode(paramsDto.ContractName, instancePath);

            if (abiBytecodeDto is null)
            {
                return NotFound("Compiled contract not found");
            }

            return Ok(new
            {
                abi = abiBytecodeDto.Abi,
                bytecode = abiBytecodeDto.Bytecode
            });
        }

        /*
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

        [Authorize(Roles = "Tester, Auditor, Deployer, Admin")]
        [HttpPost("test")]
        public async Task<IActionResult> Test(ContractParamsDto paramsDto)
        {
            if (_hh is null)
            {
                return StatusCode(500, "Internal Server Error: null hardhat executor");
            }

            if (paramsDto == null)
            {
                return BadRequest($"No reqired params send");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);

            try
            {
                // _contractService.GenerateContractCode(paramsDto, instancePath);

                var result = await _hh.TestContract(instancePath);

                return Ok(new { output = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [Authorize(Roles = "Auditor, Deployer, Admin")]
        [HttpPost("audit")]
        public async Task<IActionResult> AuditContract(ContractParamsDto paramsDto)
        {
            if (_sh is null)
            {
                return StatusCode(500, "Internal Server Error: null slither executor");
            }

            if (paramsDto == null)
            {
                return BadRequest($"No reqired params send");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);

            try
            {
                var result = await _sh.RunAnalysis(instancePath);

                return Ok("Running security audit...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
