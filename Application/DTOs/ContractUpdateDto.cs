namespace Application.DTOs
{
    public class ContractUpdateDto
    {
        public ContractParamsDto Current { get; set; } = null!;

        public string? UpdatedCode { get; set; }
        public string? UpdatedTestScript { get; set; }
        public string? UpdatedGasScript { get; set; }
    }
}
