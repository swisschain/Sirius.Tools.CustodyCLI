# Sirius.Tools.CustodyCLI

CLI utility for Sirius Custody

This utility can be used to initialize custody Guardian and Vault services. It can:

* Generate custody configuration keys pair
* Encrypt custody configuration with the public keys
* Configure specified custody instance with the encrypted configuration

## Commands

### Generate

Generates RSA keys pair to the specified output file. This command can be used to generate Customer's keys pair or Guardian keys pair.

`CustodyCli.exe generate -o|--out <key pair JSON file path>`

Example: `CustodyCli.exe generate -o keys.json`

### Encrypt

Encrypts the plain Guardian or Vault JSON configuration file with the custody configuration public key.

`CustodyCli.exe encrypt -k|--key <Custody settings RSA public key file> -i|--in <Custody settings plain JSON file> -o|--out <Custody settings encrypted file>`

Example: `CustodyCli.exe encrypt -k guardian.publickey -i guardian.json -o guardian-encrypted.json`

### Setup

Setups encrypted file to the given Guardian or Vault instance. Guardian or Vault configuration should be encrypted first with the `encrypt` command.

`CustodyCli.exe setup -u|--url <Custody settings API endpoint URL> -f|--file <Custody settings encrypted file>`

Example: `CustodyCli.exe setup -u http://localhost:5002/api/settings -f guardian-encrypted.json`
