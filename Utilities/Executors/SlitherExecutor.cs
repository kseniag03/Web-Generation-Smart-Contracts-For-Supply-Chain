using Utilities.Interfaces;

namespace Utilities.Executors
{
    public class SlitherExecutor : ISlitherExecutor
    {
        private readonly ICommandExecutor _commandExecutor;

        public SlitherExecutor(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        public Task<string> GetAnalysis(string command, string args)
        {
            throw new NotImplementedException();
        }
    }
}
