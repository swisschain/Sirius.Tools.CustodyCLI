using System;
using Sirius.Tools.CustodyCLI.Crypto;
using Xunit;

namespace Sirius.Tools.CustodyCLI.Tests.Crypto
{
    public class AsymmetricEncryptionServiceTests
    {
        [Fact]
        public void Generate_Key_Pair()
        {
            // arrange

            // act

            var keyPair = AsymmetricEncryptionService.GenerateKeyPair();
            var publicKey = AsymmetricEncryptionService.ExportPublicKey(keyPair.Public);
            var privateKey = AsymmetricEncryptionService.ExportPrivateKey(keyPair.Private);

            // assert

            Assert.True(publicKey != null && privateKey != null);
        }

        [Fact]
        public void Encrypt_Decrypt()
        {
            // arrange

            var publicKey =
                "-----BEGIN PUBLIC KEY-----\r\nMIGdMA0GCSqGSIb3DQEBAQUAA4GLADCBhwKBgQDfuR4SP1FQdSo1CMTwlsB4JDZr\r\nvyb2exLU4NA5DsEsOtNPA+bHKCfalCTO+H4TwW6f+87iNE7yjXrNNgJ+ewJOZEWw\r\n3qKVcizcbWVyOOXpBR68V0UWZ/CycWQuk3OydVKn86X0k9VnjVPDOYFgWlIivkU7\r\nI9ublilCGrAA99K2swIBAw==\r\n-----END PUBLIC KEY-----\r\n";
            var privateKey =
                "-----BEGIN RSA PRIVATE KEY-----\r\nMIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAN+5HhI/UVB1KjUI\r\nxPCWwHgkNmu/JvZ7EtTg0DkOwSw6008D5scoJ9qUJM74fhPBbp/7zuI0TvKNes02\r\nAn57Ak5kRbDeopVyLNxtZXI45ekFHrxXRRZn8LJxZC6Tc7J1UqfzpfST1WeNU8M5\r\ngWBaUiK+RTsj25uWKUIasAD30razAgEDAoGAJUmFAwqNjWjcXiwg0sPKvrCzvJ/b\r\n078tziV4CYJ1h18jN9X7y9wGpG4GIn6/rfWSb/9NJbNifcI/IjOrFRSAYhY2zCms\r\nfvXYNc4kZvHlM2Y37a6ScmNb+/th0XzE84pzfi4tNGJCB21hhBZWunVl0seCIq0W\r\nBQrCqxOaRfSBUHsCQQD4rA0FPmBaqNJOnAkQpSfKfOXevvM4fWWgh68ruPCWeJgN\r\nEkBi77IDgRnm/VmX2bmQLREevIS+P8iA+OkcaH9jAkEA5lDa31k7VHLHSe7/fOGM\r\n1VJEYRd020tk7JGWex0MoCUa0YR541mI+YmQzHuoAhV4gQtZ9pr4l1l3JBl0IGJU\r\ncQJBAKXICK4plZHF4Ym9W2BuGob97pR/TNBTmRWvyh0l9bmlurNhgEH1IVerZpn+\r\nO7qRJmAeC2nTAyl/2wCl8L2a/5cCQQCZizyU5ieNodoxSf+oll3jjC2WD6M83O3z\r\nC7mnaLMVbhHhAvvs5ltRBmCIUnABY6WrXOakZ1Bk5k9tZk1q7DhLAkAfVj55yISi\r\nEgcdS9hp94A2cWNVPT8/A0cC+AjzLz2YE5jK+4GPQ2UhKKxLnBnHUnE6vTglh2GU\r\nnoDmdR7bgzey\r\n-----END RSA PRIVATE KEY-----\r\n";

            var expectedSecret = "NHxM3k+ypDev+9kFXq1V5n3E4XQUE3ZhlItRgEDmwX8=";

            // act

            var encryptedSecret =
                AsymmetricEncryptionService.Encrypt(Convert.FromBase64String(expectedSecret), publicKey);

            var decryptedSecret = AsymmetricEncryptionService.Decrypt(encryptedSecret, privateKey);

            var actualSecret = Convert.ToBase64String(decryptedSecret);

            // assert

            Assert.Equal(expectedSecret, actualSecret);
        }
    }
}