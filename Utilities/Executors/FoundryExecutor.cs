using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class FoundryExecutor : IFoundryExecutor
    {
        private readonly ICommandExecutor _commandExecutor;

        public FoundryExecutor(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        public Task<string> GetGasReport(string command, string args)
        {
            throw new NotImplementedException();
        }

        public Task<string> TestContract(string command, string args)
        {
            throw new NotImplementedException();
        }
    }
}
