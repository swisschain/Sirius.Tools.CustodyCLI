using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;

namespace Sirius.Tools.CustodyCLI.Crypto
{
    public class AsymmetricEncryptionService
    {
        private const string PrivateKeyType = "RSA PRIVATE KEY";
        private const string PublicKeyType = "PUBLIC KEY";

        private const long PublicExponent = 3;
        private const int Strength = 1024;
        private const int Certainty = 25;

        private static readonly SecureRandom SecureRandom = new();

        public static byte[] GenerateSignature(byte[] data, string privateKey)
        {
            var signer = new RsaDigestSigner(new Sha256Digest());
            signer.Init(true, GetPrivateKeyParameters(privateKey));
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public static bool VerifySignature(byte[] data, byte[] signature, string publicKey)
        {
            var signer = new RsaDigestSigner(new Sha256Digest());
            signer.Init(false, GetPublicKeyParameters(publicKey));
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }

        public static byte[] Encrypt(byte[] data, string publicKey)
        {
            var cipher = new Pkcs1Encoding(new RsaEngine());
            cipher.Init(true, GetPublicKeyParameters(publicKey));
            return cipher.ProcessBlock(data, 0, data.Length);
        }

        public static byte[] Decrypt(byte[] data, string privateKey)
        {
            var cipher = new Pkcs1Encoding(new RsaEngine());
            cipher.Init(false, GetPrivateKeyParameters(privateKey));
            return cipher.ProcessBlock(data, 0, data.Length);
        }

        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var keyPairGenerator = new RsaKeyPairGenerator();

            var parameters =
                new RsaKeyGenerationParameters(
                    BigInteger.ValueOf(PublicExponent), SecureRandom, Strength, Certainty);

            keyPairGenerator.Init(parameters);

            var keyPair = keyPairGenerator.GenerateKeyPair();

            return new AsymmetricCipherKeyPair(keyPair.Public, keyPair.Private);
        }

        public static string ExportPublicKey(AsymmetricKeyParameter keyParameter)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyParameter);
            var encodedKey = publicKeyInfo.ToAsn1Object().GetEncoded();

            using var writer = new StringWriter();
            var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(writer);
            pemWriter.WriteObject(new PemObject(PublicKeyType, encodedKey));

            return writer.ToString();
        }

        public static string ExportPrivateKey(AsymmetricKeyParameter privateKeyParameter)
        {
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParameter);

            using var writer = new StringWriter();
            var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(writer);
            pemWriter.WriteObject(new PemObject(PrivateKeyType, privateKeyInfo.GetEncoded()));

            return writer.ToString();
        }

        private static AsymmetricKeyParameter GetPrivateKeyParameters(string privateKey)
        {
            var bytes = Encoding.UTF8.GetBytes(privateKey);
            using var memStream = new MemoryStream(bytes);
            using var textReader = new StreamReader(memStream);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(textReader);

            return PrivateKeyFactory.CreateKey(pemReader.ReadPemObject().Content);
        }

        private static RsaKeyParameters GetPublicKeyParameters(string publicKey)
        {
            var bytes = Encoding.UTF8.GetBytes(publicKey);
            using var memStream = new MemoryStream(bytes);
            using var textReader = new StreamReader(memStream);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(textReader);
            var pemObject = (RsaKeyParameters) pemReader.ReadObject();

            return new RsaKeyParameters(false, pemObject.Modulus, pemObject.Exponent);
        }
    }
}