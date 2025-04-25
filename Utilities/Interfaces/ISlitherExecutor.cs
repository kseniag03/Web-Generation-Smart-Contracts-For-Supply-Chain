namespace Utilities.Interfaces
{
    public interface ISlitherExecutor
    {
        Task<string> RunAnalysis(string instancePath);
    }
}
