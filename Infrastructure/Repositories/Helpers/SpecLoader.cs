using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Application.Specifications.Yaml;

namespace Infrastructure.Repositories.Helpers
{
    public static class SpecLoader
    {
        public static dynamic LoadObjectFromYaml(string path)
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<ContractModel>(yaml);
        }

        public static ContractModel LoadModelFromYaml(string path)
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<ContractModel>(yaml);
        }
    }
}
