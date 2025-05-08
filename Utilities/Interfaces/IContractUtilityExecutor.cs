namespace Utilities.Interfaces
{
    public interface IContractUtilityExecutor
    {
        Task<string> SetupInstanceEnvironment(string instancePath);
    }
}
