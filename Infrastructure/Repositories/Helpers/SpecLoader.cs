using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Application.Specifications.Yaml;

namespace Infrastructure.Repositories.Helpers
{
    public static class SpecLoader
    {
        public static dynamic LoadObjectFromYamlFile(string path)
        {
            var yaml = File.ReadAllText(path);

            return DeserializeContractModelFromYaml(yaml);
        }

        public static async Task<dynamic> LoadObjectFromYamlFileAsync(string path)
        {
            var yaml = await File.ReadAllTextAsync(path);

            return LoadModelFromYaml(yaml);
        }

        public static ContractModel LoadModelFromYamlFile(string path)
        {
            var yaml = File.ReadAllText(path);

            return DeserializeContractModelFromYaml(yaml);
        }

        public static async Task<ContractModel> LoadModelFromYamlFileAsync(string path)
        {
            var yaml = await File.ReadAllTextAsync(path);

            return LoadModelFromYaml(yaml);
        }

        public static ContractModel LoadModelFromYaml(string yaml)
        {
            return DeserializeContractModelFromYaml(yaml);
        }

        private static ContractModel DeserializeContractModelFromYaml(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<ContractModel>(yaml);
        }
    }
}
