using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class MappingModel
    {
        public MappingModel()
        {
            Name = "Name";
            Visibility = AppConstants.DefaultStateVariableVisibility;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "key")]
        public string? Key { get; set; }

        [YamlMember(Alias = "value")]
        public string? Value { get; set; }

        [YamlMember(Alias = "visibility")]
        public string Visibility { get; set; }
    }
}
