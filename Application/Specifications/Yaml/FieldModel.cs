using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class FieldModel
    {
        public FieldModel()
        {
            Name = AppConstants.DefaultFieldName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }
    }
}
