namespace Utilities.Interfaces
{
    public interface ICommandExecutor
    {
        Task<string> ExecuteCommandAsync(string command, string args);
    }

    public interface IHardhatExecutor
    {
        Task<string> TestContract(string command, string args);
        Task<string> DeployContract(string command, string args);
        Task<string> VerifyContract(string command, string args);
    }

    public interface IFoundryExecutor
    {
        Task<string> TestContract(string command, string args);
        Task<string> GetGasReport(string command, string args);
    }

    public interface ISlitherExecutor
    {
        Task<string> GetAnalysis(string command, string args);
    }

    public interface IMythrilExecutor
    {
        Task<string> GetAnalysis(string command, string args);
    }
}
