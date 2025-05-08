using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class ContractRoleModel
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
