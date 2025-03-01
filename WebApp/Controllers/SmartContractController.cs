using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class SmartContractController : ControllerBase
    {
        [HttpGet("generate")]
        public IActionResult GenerateContract(string name)
        {
            return Ok($"Contract {name} generated!");
        }
    }
}
