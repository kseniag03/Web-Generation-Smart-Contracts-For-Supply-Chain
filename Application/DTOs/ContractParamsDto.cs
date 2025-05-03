namespace Application.DTOs
{
    public class ContractParamsDto
    {
        public required string ContractName { get; set; }
        public string? ApplicationArea { get; set; }
        public string? UintType { get; set; }
        public bool EnableEvents { get; set; }
        public bool IncludeVoidLabel { get; set; }
        public string? TargetOs { get; set; }

    }
}
