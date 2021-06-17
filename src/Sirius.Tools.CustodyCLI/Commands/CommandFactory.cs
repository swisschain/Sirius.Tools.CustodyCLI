using System;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class CommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider helper)
        {
            _serviceProvider = helper;
        }

        public ICommand CreateCommand(Func<IServiceProvider, ICommand> createFunc)
        {
            var command = createFunc(_serviceProvider);

            return command;
        }
    }
}