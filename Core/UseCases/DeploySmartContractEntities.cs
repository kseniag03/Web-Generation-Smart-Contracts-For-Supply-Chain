using System.Text.Json;

namespace Core.UseCases
{
    public class DeploySmartContractEntities
    {
        public JsonElement? Abi { get; set; }
        public string? Bytecode { get; set; }
    }
}
