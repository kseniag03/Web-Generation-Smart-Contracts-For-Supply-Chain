using Application.Common;

namespace Application.DTOs
{
    public class ContractParamsDto
    {
        public ContractParamsDto()
        {
            Area = AppConstants.DefaultContractAreaPath;
            LayoutYaml = AppConstants.DefaultYamlContent;
        }

        public string Area { get; set; }

        public string LayoutYaml { get; set; }
    }
}
