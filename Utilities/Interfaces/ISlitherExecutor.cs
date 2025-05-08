namespace Utilities.Interfaces
{
    public interface ISlitherExecutor : IContractUtilityExecutor
    {
        Task<string> RunAnalysis(string instancePath);
    }
}
