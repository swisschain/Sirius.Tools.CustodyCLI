namespace Sirius.Tools.CustodyCLI.Commands
{
    public class InitVaultDbCommandRegistration : InitDbCommandRegistrationBase
    {
        public InitVaultDbCommandRegistration(CommandFactory factory) : 
            base(factory,
                "Initializes Vault DB.",
                "sirius_vault_user")
        {
        }
    }
}
