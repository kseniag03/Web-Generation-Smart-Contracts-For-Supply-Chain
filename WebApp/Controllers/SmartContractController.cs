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

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContractAsync(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                async instancePath =>
                {
                    var code = _contractService.GenerateContractCode(paramsDto, instancePath);

                    if (!string.IsNullOrEmpty(code))
                    {
                        await _hh.SetupInstanceEnvironment(instancePath);
                    }

                    return new { paramsDto.Area, code, instancePath };
                },
                result => Ok(result)
            );
        }

        [HttpGet("code")]
        public async Task<IActionResult> GetContractCode(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var code = _contractService.GetContractCode(paramsDto.Area, instancePath);

                    return Task.FromResult(code);
                },
                code => string.IsNullOrEmpty(code)
                    ? NotFound("Contract code not found")
                    : Ok(new { paramsDto.Area, code })
            );
        }


        [Authorize(Roles = "deployer,admin")]
        [HttpGet("address")]
        public async Task<IActionResult> GetDeployedContractAddress(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var address = _contractService.GetDeployedContractAddress(paramsDto.Area, instancePath);

                    return Task.FromResult(address);
                },
                address => string.IsNullOrEmpty(address)
                    ? NotFound("Contract is not deployed")
                    : Ok(new { paramsDto.Area, address })
            );
        }

        [Authorize(Roles = "deployer,admin")]
        [HttpGet("abi-bytecode")]
        public async Task<IActionResult> GetAbiAndBytecode(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var dto = _contractService.GetContractAbiBytecode(paramsDto.Area, instancePath);

                    return Task.FromResult(dto);
                },
                dto => dto is null
                    ? NotFound("Compiled contract not found")
                    : Ok(new { abi = dto.Abi, bytecode = dto.Bytecode })
            );
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


        [HttpPost("compile")]
        public async Task<IActionResult> CompileContract(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                _hh.CompileContract,
                result => Ok(new { output = result }),
                true,
                _hh
            );
        }

        [Authorize(Roles = "tester,auditor,deployer,admin")]
        [HttpPost("test")]
        public async Task<IActionResult> TestContract(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                async instancePath =>
                {
                    await _hh.CompileContract(instancePath);

                    return await _hh.TestContract(instancePath);
                },
                result => Ok(new { output = result }),
                true,
                _hh
            );
        }

        [Authorize(Roles = "tester,auditor,deployer,admin")]
        [HttpPost("gas-report")]
        public async Task<IActionResult> GetContractGasReport(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath => _fh.GetGasReport(instancePath),
                result => Ok(new { output = result }),
                true,
                _fh
            );
        }

        [Authorize(Roles = "auditor,deployer,admin")]
        [HttpPost("audit")]
        public async Task<IActionResult> AuditContract(ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                _sh.RunAnalysis,
                result => Ok("Running security audit..."),
                true,
                _sh
            );
        }

        private async Task<IActionResult> ContractAction<T>(
            ContractParamsDto paramsDto,
            Func<string, Task<T>> action,
            Func<T, IActionResult> resultHandler,
            bool requireExecutor = false,
            IContractUtilityExecutor? executor = null)
        {
            if (requireExecutor && executor is null)
            {
                return StatusCode(500, "Internal Server Error: required executor is null");
            }

            if (paramsDto == null || string.IsNullOrEmpty(paramsDto.Area))
            {
                return BadRequest("Missing contract parameter");
            }

            var instancePath = ContractPathHelper.ComputeInstancePath(paramsDto);

            if (instancePath is null)
            {
                return BadRequest("Error generating instance path");
            }

            try
            {
                var result = await action(instancePath);

                return resultHandler(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
