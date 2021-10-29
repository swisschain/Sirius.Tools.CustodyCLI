using System;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public static class ErrorRedirect
    {
        private const int UnknownErrorExitCode = 10;

        private const int OptionValidationFailedExitCode = 1;

        public static int RedirectErrorToStandardError(Func<int> func)
        {
            try
            {
                return func();
            }
            catch (OptionInvalidException exception)
            {
                Console.Error.WriteLine(exception.Message);
                return OptionValidationFailedExitCode;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                return UnknownErrorExitCode;
            }
        }
    }
}
