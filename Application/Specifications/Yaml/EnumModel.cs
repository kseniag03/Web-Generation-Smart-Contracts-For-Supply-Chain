using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class EnumModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "values")]
        public List<string> Values { get; set; } = new();
    }
}
