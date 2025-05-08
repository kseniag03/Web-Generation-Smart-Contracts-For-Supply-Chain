using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class StructModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "fields")]
        public List<FieldModel> Fields { get; set; } = new();
    }
}
