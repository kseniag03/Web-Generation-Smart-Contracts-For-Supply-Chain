using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class ConstantModel
    {
        public ConstantModel()
        {
            Name = AppConstants.DefaultConstantName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "value")]
        public object? Value { get; set; }
    }
}
