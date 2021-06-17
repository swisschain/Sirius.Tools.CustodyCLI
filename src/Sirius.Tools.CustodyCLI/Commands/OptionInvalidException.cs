using System;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class OptionInvalidException : Exception
    {
        public OptionInvalidException(string message) : base(message)
        {
        }
    }
}