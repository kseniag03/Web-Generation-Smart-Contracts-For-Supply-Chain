using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class FunctionModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "visibility")]
        public string Visibility { get; set; }

        [YamlMember(Alias = "restrictedTo")]
        public List<string> RestrictedTo { get; set; } = new();

        [YamlMember(Alias = "args")]
        public List<FieldModel> Args { get; set; } = new();

        [YamlMember(Alias = "returnParams")]
        public List<FieldModel> ReturnParams { get; set; } = new();

        [YamlMember(Alias = "body")]
        public string Body { get; set; }

        [YamlMember(Alias = "emitEvents")]
        public List<string> EmitEvents { get; set; } = new();
    }
}
