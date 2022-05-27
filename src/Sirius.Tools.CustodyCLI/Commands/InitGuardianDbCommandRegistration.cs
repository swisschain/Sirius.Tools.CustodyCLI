namespace Sirius.Tools.CustodyCLI.Commands
{

    public class InitGuardianDbCommandRegistration : InitDbCommandRegistrationBase
    {
        public InitGuardianDbCommandRegistration(CommandFactory factory) :
            base(factory,
                "Initializes Guardian DB.",
                "sirius_guardian_user")
        {
        }
    }
}
