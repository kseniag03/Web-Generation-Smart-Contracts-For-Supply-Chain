using Application.Specifications.Yaml;

namespace Application.Interfaces
{
    public interface IContractModelProvider
    {
        ContractModel GetContractModelFromYaml(string yaml);
    }
}
