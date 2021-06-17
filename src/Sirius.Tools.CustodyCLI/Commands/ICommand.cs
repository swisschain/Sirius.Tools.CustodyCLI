using System.Threading.Tasks;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public interface ICommand
    {
        Task<int> ExecuteAsync();
    }
}