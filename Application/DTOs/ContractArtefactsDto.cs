using Core.Enums;

namespace Application.DTOs
{
    public class ContractArtefactsDto
    {
        public ContractArtefactsDto()
        {
            Status = ContractStatus.None;
        }

        public required string InstancePath { get; set; }
        public string? UserLogin { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? TestScript { get; set; }
        public string? TestGasScript { get; set; }
        public string? Address { get; set; }
        public ContractStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
