using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class ContractRoleModel
    {
        public ContractRoleModel()
        {
            Name = AppConstants.DefaultRoleName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
