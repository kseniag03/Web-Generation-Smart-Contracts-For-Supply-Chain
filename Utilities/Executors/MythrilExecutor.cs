using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class MythrilExecutor : IMythrilExecutor
    {
        private readonly ICommandExecutor _commandExecutor;

        public MythrilExecutor(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        public Task<string> GetAnalysis(string command, string args)
        {
            throw new NotImplementedException();
        }
    }
}
