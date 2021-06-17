using System;
using System.Text;
using Sirius.Tools.CustodyCLI.Crypto;
using Xunit;

namespace Sirius.Tools.CustodyCLI.Tests.Crypto
{
    public class SymmetricEncryptionServiceTests
    {
        [Fact]
        public void Generate_Secret()
        {
            // arrange

            // act

            var secret = Convert.ToBase64String(SymmetricEncryptionService.GenerateKey());
            var nonce = Convert.ToBase64String(SymmetricEncryptionService.GenerateNonce());

            // assert

            Assert.True(secret != null && nonce != null);
        }

        [Fact]
        public void Encrypt_Decrypt()
        {
            // arrange

            var expectedMessage =
                "{\"amount\":\"3.9812\",\"asset\":{\"assetAddress\":\"0x3A9BC420a42D4386D1A84CC560e7324779D86734\",\"assetId\":\"100001\",\"symbol\":\"ETH\"},\"blockchainId\":\"ethereum-ropsten\",\"blockchainProtocolId\":\"ethereum\",\"clientContext\":{\"userId\":\"1000045\",\"apiKeyId\":\"4000762\",\"accountReferenceId\":\"Mr. White\",\"withdrawalReferenceId\":\"Mr. Red\",\"ip\":\"10.0.25.179\",\"timestamp\":\"2020-09-30T13:41:12.209060300Z\"},\"destination\":{\"address\":\"0x1A9BC420a42D4386D1A84CC560e7324779D86734\",\"name\":\"No name\",\"group\":\"1000457\",\"tag\":\"this is a text tag value\",\"tagType\":\"text\"},\"feeLimit\":\"0.657\",\"networkType\":\"test\",\"operationId\":\"D2B5B7E5-15CF-44C8-8C3E-361B421DE671\",\"source\":{\"address\":\"0x4A9BC420a42D4386D1A84CC560e7324779D86734\",\"name\":null,\"group\":\"1000458\"}}";

            var secret = "8HNWdlJmzeGlPbGDsmC0nppscMcm3uC0fdZFywWAMEU=";
            var nonce = "uphBIuXed6wj3bqP6Ezq7Q==";

            // act

            var encryptedMessage = SymmetricEncryptionService.Encrypt(Encoding.UTF8.GetBytes(expectedMessage),
                Convert.FromBase64String(secret), Convert.FromBase64String(nonce));

            var decryptedMessage = SymmetricEncryptionService.Decrypt(encryptedMessage,
                Convert.FromBase64String(secret), Convert.FromBase64String(nonce));

            var actualMessage = Encoding.UTF8.GetString(decryptedMessage);

            // assert

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}