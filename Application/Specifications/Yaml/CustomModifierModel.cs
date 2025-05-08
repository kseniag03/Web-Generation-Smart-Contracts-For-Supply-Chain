using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class CustomModifierModel
    {
        public CustomModifierModel()
        {
            Name = AppConstants.DefaultModifierName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "body")]
        public string? Body { get; set; }

        [YamlMember(Alias = "args")]
        public List<FieldModel> Args { get; set; } = new();
    }
}
