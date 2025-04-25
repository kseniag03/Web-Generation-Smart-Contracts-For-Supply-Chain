using System.Text.Json;

namespace Application.DTOs
{
    public class AbiBytecodeDto
    {
        public JsonElement? Abi { get; set; }
        public string? Bytecode { get; set; }
    }
}
