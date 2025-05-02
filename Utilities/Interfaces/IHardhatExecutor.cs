namespace Utilities.Interfaces
{
    public interface IHardhatExecutor
    {
        Task<string> SetupInstanceEnvironment(string instancePath);
        Task<string> CompileContract(string instancePath);
        Task<string> TestContract(string instancePath);
    }
}
