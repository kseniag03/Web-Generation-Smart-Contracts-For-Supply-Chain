using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class MappingModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "key")]
        public string Key { get; set; }

        [YamlMember(Alias = "value")]
        public string Value { get; set; }

        [YamlMember(Alias = "visibility")]
        public string Visibility { get; set; }
    }
}
