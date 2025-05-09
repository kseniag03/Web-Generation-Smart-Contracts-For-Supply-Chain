using Application.Specifications.Yaml;
using Application.Interfaces;

namespace Infrastructure.Repositories.Helpers
{
    public class ContractModelProvider : IContractModelProvider
    {
        public ContractModel GetContractModelFromYaml(string yaml)
        {
            return SpecLoader.LoadModelFromYaml(yaml);
        }
    }
}
