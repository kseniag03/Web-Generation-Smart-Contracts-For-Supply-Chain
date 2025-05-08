using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class VariableModel
    {
        public VariableModel()
        {
            Name = AppConstants.DefaultVariableName;
            Visibility = AppConstants.DefaultStateVariableVisibility;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "visibility")]
        public string Visibility { get; set; }
    }
}
