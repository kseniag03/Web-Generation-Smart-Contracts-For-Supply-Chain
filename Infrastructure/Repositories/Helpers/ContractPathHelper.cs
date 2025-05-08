using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.DTOs;

namespace Infrastructure.Repositories.Helpers
{
    public static class ContractPathHelper
    {
        public static string ComputeInstancePath(ContractParamsDto dto)
        {
            using var sha256 = SHA256.Create();
            var json = JsonSerializer.Serialize(dto);
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            var instancePath = Path.Combine("Instances", hash);

            Directory.CreateDirectory(instancePath);

            return instancePath;
        }
    }
}
