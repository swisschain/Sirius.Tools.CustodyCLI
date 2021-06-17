using System;
using System.Threading.Tasks;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public static class ErrorRedirect
    {
        private const int UnknownErrorExitCode = 10;

        private const int OptionValidationFailedExitCode = 1;

        public static async Task<int> RedirectErrorToStandardErrorAsync(Func<Task<int>> func)
        {
            try
            {
                return await func();
            }
            catch (OptionInvalidException exception)
            {
                await Console.Error.WriteLineAsync(exception.Message);

                return OptionValidationFailedExitCode;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);

                return UnknownErrorExitCode;
            }
        }

        public static int RedirectErrorToStandardError(Func<int> func)
        {
            try
            {
                return func();
            }
            catch (OptionInvalidException e)
            {
                Console.Error.WriteLine(e.Message);

                return OptionValidationFailedExitCode;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);

                return UnknownErrorExitCode;
            }
        }
    }
}