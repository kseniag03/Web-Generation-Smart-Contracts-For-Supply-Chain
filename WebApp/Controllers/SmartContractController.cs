using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.DTOs;
using Application.Services;
using Utilities.Interfaces;
using System.ComponentModel.DataAnnotations;
using Humanizer;

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

        [HttpPost("setup")]
        public async Task<IActionResult> SetupTemplate([FromBody] ContractParamsDto paramsDto)
        {
            if (paramsDto == null || string.IsNullOrEmpty(paramsDto.Area))
            {
                return BadRequest("Missing contract parameter");
            }

            try
            {
                var yaml = await _contractService.LoadYamlTemplate(paramsDto);

                return Ok(new { yaml });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("generate")]
        public Task<IActionResult> GenerateContract([FromBody] ContractParamsDto paramsDto)
        {
            return ContractAction(
                paramsDto,
                async instancePath =>
                {
                    var artefacts = await _contractService.GenerateContractCode(paramsDto, instancePath);

                    if (artefacts is null)
                    {
                        return default;
                    }

                    try
                    {

                        await _hh.SetupInstanceEnvironment(instancePath);
                        await _fh.SetupInstanceEnvironment(instancePath);
                        await _sh.SetupInstanceEnvironment(instancePath);
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;

                        Console.WriteLine($"Error via local debug without docker: {error}");
                    }

                    return new
                    {
                        paramsDto.Area,
                        code = artefacts.Code,
                        testScript = artefacts.TestScript,
                        gasReport = artefacts.TestGasScript,
                        instancePath
                    };
                },
                result => Ok(result),
                requireExecutor: true,
                executor: _hh
            );
        }

        [HttpGet("code")]
        public async Task<IActionResult> GetContractCode([FromBody] ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var code = _contractService.GetContractCode(instancePath);

                    return Task.FromResult(code);
                },
                code => string.IsNullOrEmpty(code)
                    ? NotFound("Contract code not found")
                    : Ok(new { paramsDto.Area, code })
            );
        }

        [HttpPost("compile")]
        public async Task<IActionResult> CompileContract([FromBody] ContractParamsDto paramsDto)
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
        public async Task<IActionResult> TestContract([FromBody] ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                async instancePath =>
                {
                    _ = await _hh.CompileContract(instancePath);

                    return await _hh.TestContract(instancePath);
                },
                result => Ok(new { output = result }),
                true,
                _hh
            );
        }

        [Authorize(Roles = "tester,auditor,deployer,admin")]
        [HttpPost("gas-report")]
        public async Task<IActionResult> GetContractGasReport([FromBody] ContractParamsDto paramsDto)
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
        public async Task<IActionResult> AuditContract([FromBody] ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                _sh.RunAnalysis,
                result => Ok(new { output = result }),
                true,
                _sh
            );
        }

        [Authorize(Roles = "deployer,admin")]
        [HttpGet("address")]
        public async Task<IActionResult> GetDeployedContractAddress([FromBody] ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var address = _contractService.GetDeployedContractAddress(instancePath);

                    return Task.FromResult(address);
                },
                address => string.IsNullOrEmpty(address)
                    ? NotFound("Contract is not deployed")
                    : Ok(new { paramsDto.Area, address })
            );
        }

        [Authorize(Roles = "deployer,admin")]
        [HttpGet("abi-bytecode")]
        public async Task<IActionResult> GetAbiAndBytecode([FromBody] ContractParamsDto paramsDto)
        {
            return await ContractAction(
                paramsDto,
                instancePath =>
                {
                    var dto = _contractService.GetContractAbiBytecode(instancePath);

                    return Task.FromResult(dto);
                },
                dto => dto is null
                    ? NotFound("Compiled contract not found")
                    : Ok(new { abi = dto.Abi, bytecode = dto.Bytecode })
            );
        }

        /*
        [HttpGet("contract-info")]
        public async Task<IActionResult> GetContractInfo(string contractAddress)
        {
            var contractInfo = await _contractService.GetContractInfoAsync(contractAddress);

            if (contractInfo == null)
                return NotFound("Contract info not found.");

            return Ok(contractInfo);
        }*/

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

            try
            {
                var instancePath = _contractService.GetInstancePath(paramsDto);

                if (instancePath is null)
                {
                    return BadRequest("Error generating instance path");
                }

                var result = await action(instancePath);

                Console.WriteLine("=== ContractAction result ===");
                Console.WriteLine(result == null ? "null" : $"[{result}]");

                if (result is null)
                {
                    return BadRequest("Action returned null");
                }

                return resultHandler(result);
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                return BadRequest($"YAML syntax error at line {ex.Start.Line}, col {ex.Start.Column}: {ex.Message}");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
