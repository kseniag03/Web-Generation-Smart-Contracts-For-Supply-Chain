using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class StructModel
    {
        public StructModel()
        {
            Name = AppConstants.DefaultStructName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "fields")]
        public List<FieldModel> Fields { get; set; } = new();
    }
}
