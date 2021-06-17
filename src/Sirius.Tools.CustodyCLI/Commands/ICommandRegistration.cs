using Microsoft.Extensions.CommandLineUtils;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public interface ICommandRegistration
    {
        void StartExecution(CommandLineApplication lineApplication);
    }
}