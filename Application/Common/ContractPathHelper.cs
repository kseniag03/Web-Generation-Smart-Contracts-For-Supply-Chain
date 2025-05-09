using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Specifications.Yaml;

namespace Application.Common
{
    public static class ContractPathHelper
    {
        public static string ComputeInstancePath(string area, ContractModel model, string solutionRoot)
        {
            using var sha256 = SHA256.Create();
            var json = JsonSerializer.Serialize(new
            {
                area,
                model
            });

            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            var instancePath = Path.Combine(solutionRoot, "Instances", hash);

            Directory.CreateDirectory(instancePath);

            return instancePath;
        }
    }
}
