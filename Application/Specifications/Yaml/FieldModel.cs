using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class FieldModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string Type { get; set; }
    }
}
