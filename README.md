# Sirius.Tools.CustodyCLI

CLI utility for Sirius Custody

This utility can be used to initialize custody Guardian and Vault services. It can:

* Generate custody configuration keys pair
* Encrypt custody configuration with the public keys
* Configure specified custody instance with the encrypted configuration

## Commands

### Generate

Generates AES|RSA keys to the specified output file. This command can be used to generate Customer's keys pair or Guardian keys pair.

`CustodyCli.exe generate -o|--out <key pair file name without extension> -t|--type <key type (aes|rsa)>`

Example: `CustodyCli.exe generate -o rsa_key -t rsa` 

Example: `CustodyCli.exe generate -o aes_key -t aes`

### Encrypt

Encrypts the plain Guardian or Vault JSON configuration file with the custody configuration public key.

`CustodyCli.exe encrypt -k|--key <Custody settings RSA public key file> -u|--url <Custody URL> -i|--in <Custody settings plain JSON file> -o|--out <Custody settings encrypted file>`

Example: `CustodyCli.exe encrypt -k guardian.publickey -i guardian.json -o guardian-encrypted.json`
Example: `CustodyCli.exe encrypt -u http://localhost:5000/ -i guardian.json -o guardian-encrypted.json`

### Setup

Setups encrypted file to the given Guardian or Vault instance. Guardian or Vault configuration should be encrypted first with the `encrypt` command.

`CustodyCli.exe setup -u|--url <Custody URL> -f|--file <Custody settings encrypted file>`

Example: `CustodyCli.exe setup -u http://localhost:5000/ -f guardian-encrypted.json`

### Run in docker

Run docker

`docker run --name dn -it mcr.microsoft.com/dotnet/sdk:5.0`

If you plan to get git repository by ssh install Vim and OpenSSH, add ssh key to /tmp/key file and load it

`chmod 400 /tmp/key && eval $(ssh-agent -s ) && ssh-add /tmp/key`

Clone repository and run build

`git clone git@github.com:swisschain/Sirius.Tools.CustodyCLI.git && cd Sirius.Tools.CustodyCLI/ && dotnet build`

Go to bin directory

`cd src/Sirius.Tools.CustodyCLI/bin/Debug/net5.0/`

Run initialization

`dotnet CustodyCli.dll setup -u|--url <Custody URL> -f|--file <Custody settings encrypted file>`
