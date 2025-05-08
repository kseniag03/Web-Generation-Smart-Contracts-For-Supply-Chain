using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class ContractModel
    {
        [YamlMember(Alias = "contractName")]
        public string ContractName { get; set; } = "DefaultName";

        [YamlMember(Alias = "spdx")]
        public string Spdx { get; set; } = "MIT";

        [YamlMember(Alias = "pragma")]
        public string Pragma { get; set; } = "^0.8.22";

        [YamlMember(Alias = "imports")]
        public List<string> Imports { get; set; } = new();

        [YamlMember(Alias = "inheritance")]
        public List<string> Inheritance { get; set; } = new();

        [YamlMember(Alias = "enableAccessControl")]
        public bool EnableAccessControl { get; set; } = false;

        [YamlMember(Alias = "idType")]
        public string IdType { get; set; } = "uint256";

        [YamlMember(Alias = "constants")]
        public List<ConstantModel> Constants { get; set; } = new();

        [YamlMember(Alias = "enums")]
        public List<EnumModel> Enums { get; set; } = new();

        [YamlMember(Alias = "structs")]
        public List<StructModel> Structs { get; set; } = new();

        [YamlMember(Alias = "variables")]
        public List<VariableModel> Variables { get; set; } = new();

        [YamlMember(Alias = "mappings")]
        public List<MappingModel> Mappings { get; set; } = new();

        [YamlMember(Alias = "events")]
        public List<EventModel> Events { get; set; } = new();

        [YamlMember(Alias = "roles")]
        public List<ContractRoleModel> Roles { get; set; } = new();

        [YamlMember(Alias = "customModifiers")]
        public List<CustomModifierModel> CustomModifiers { get; set; } = new();

        [YamlMember(Alias = "functions")]
        public List<FunctionModel> Functions { get; set; } = new();
    }
}
