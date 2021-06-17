namespace Sirius.Tools.CustodyCLI.Contracts
{
    public class EncryptedSettings
    {
        public string Key { get; set; }

        public string Nonce { get; set; }

        public string Settings { get; set; }
    }
}