using System.ComponentModel.DataAnnotations;
using Application.Common;
using Core.Enums;

namespace Application.DTOs
{
    public class ContractParamsDto
    {
        public ContractParamsDto()
        {
            Area = AppConstants.DefaultContractAreaPath;
            LayoutYaml = AppConstants.DefaultYamlContent;
        }

        [Required]
        [EnumDataType(typeof(ContractArea),
            ErrorMessage = "Area must be one of: Empty, IoT, Pharmaceutics")]
        public string Area { get; set; }

        public string LayoutYaml { get; set; }
    }
}
