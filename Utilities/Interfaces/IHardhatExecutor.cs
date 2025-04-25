namespace Utilities.Interfaces
{
    public interface IHardhatExecutor
    {
        Task<string> TestContract(string instancePath);
        Task<string> CompileContract(string instancePath);
    }
}
