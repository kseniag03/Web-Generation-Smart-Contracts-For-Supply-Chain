using System.ComponentModel.DataAnnotations;
using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class ContractModel : IValidatableObject
    {
        public ContractModel()
        {
            ContractName = AppConstants.DefaultContractName;
            Spdx = AppConstants.DefaultSpdx;
            Pragma = AppConstants.DefaultPragma;
            IdType = AppConstants.DefaultIdType;
        }

        [Required(ErrorMessage = "The name of the contract is required")]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Only Latin letters, numbers, and '_' are allowed.")]
        [YamlMember(Alias = "contractName")]
        public string ContractName { get; set; }

        [YamlMember(Alias = "spdx")]
        public string Spdx { get; set; }

        [YamlMember(Alias = "pragma")]
        public string Pragma { get; set; }

        [YamlMember(Alias = "imports")]
        public List<string> Imports { get; set; } = new();

        [YamlMember(Alias = "inheritance")]
        public List<string> Inheritance { get; set; } = new();

        [YamlMember(Alias = "enableAccessControl")]
        public bool EnableAccessControl { get; set; } = false;

        [YamlMember(Alias = "idType")]
        public string IdType { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(
                ContractName,
                new ValidationContext(this) { MemberName = nameof(ContractName) },
                results
            );

            return results;
        }
    }
}
