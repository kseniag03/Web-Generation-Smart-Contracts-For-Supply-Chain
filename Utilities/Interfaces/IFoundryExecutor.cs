namespace Utilities.Interfaces
{
    public interface IFoundryExecutor
    {
        Task<string> GetGasReport(string instancePath, string args = "");
    }
}
