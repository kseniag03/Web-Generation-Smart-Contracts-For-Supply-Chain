using Application.Common;

namespace Application.Interfaces
{
    public interface ITemplateRepository
    {
        Task<string> LoadAreaModelTemplate(string area);
        Task<string> GenerateContractCode(string area, string yaml, string instancePath);
        Task<string> GenerateContractTestScript(string area, string yaml, string instancePath);
        Task<string> GenerateContractTestGasScript(string area, string yaml, string instancePath);
        string ExtractContractName(string yaml);
        Task UpdateFileContent(string area, string yaml, string instancePath, string? content, string sbnType = AppConstants.DefaultSbnType);
    }
}
