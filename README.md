# Sirius.Tools.CustodyCLI

CLI utility for Sirius Custody

This utility can be used to initialize custody Guardian and Vault services. It can:

* Generate custody configuration keys pair
* Encrypt custody configuration with the public keys
* Configure specified custody instance with the encrypted configuration

## Build

`.Net 7.0` SDK is required in order to build and run Custody CLI

If you want to get git repository by ssh install Vim and OpenSSH, add ssh key to `/tmp/key` file and load it

`chmod 400 /tmp/key && eval $(ssh-agent -s ) && ssh-add /tmp/key`

Clone repository and run build

```bash
git clone git@github.com:swisschain/Sirius.Tools.CustodyCLI.git
cd Sirius.Tools.CustodyCLI
dotnet build
```

Go to the `bin` directory:

`cd src/Sirius.Tools.CustodyCLI/bin/Debug/net5.0/`

You can run commands now

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

### Initialize Guardian database

Initializes Guardian database. Creates required roles and DB. Both Vault and Guardian can use the same DB and user role.

`CustodyCli.exe initguardiandb -c|--conn <DB connection string> -p|--upass <DB user password> [-d|--dbname <DB name. Default: custody>] [-g|--group <DB users group name. Default: sirius_users_group. Default: sirius_guardian_user>] [-u|--uname <DB user name>] [--connlimit <DB user connections limit. Default: 50>] [--dbconnlimit <DB connections limit. Default: -1>]`

There are also optional parameters, see `CustodyCli.exe initguardiandb --help` for all options.

Example: `CustodyCli.exe initguardiandb -c "Server=localhost;Database=postgres;Port=5432;User Id=admin;Password=admin;SSL Mode=Prefer;Root Certificate=cert.pem" -p "0123456789ABCdef"`

### Initialize Vault database

Initializes Vault database. Creates required roles and DB. Both Vault and Guardian can use the same DB and user role.

`CustodyCli.exe initvaultdb -c|--conn <DB connection string> -p|--upass <DB user password> [-d|--dbname <DB name. Default: custody>] [-g|--group <DB users group name. Default: sirius_users_group. Default: sirius_vault_user>] [-u|--uname <DB user name>] [--connlimit <DB user connections limit. Default: 50>] [--dbconnlimit <DB connections limit. Default: -1>]`

There are also optional parameters, see `CustodyCli.exe initvaultdb --help` for all options.

Example: `CustodyCli.exe initvaultdb -c "Server=localhost;Database=postgres;Port=5432;User Id=admin;Password=admin;SSL Mode=Prefer;Root Certificate=cert.pem" -p "0123456789ABCdef"`

### Vault root key шnitialization

Initializes Vault root key while first start. Sends root key configuration to the Vault that starts the initialization process.

`CustodyCli.exe initvaultrootkey -u|--url <Vault URL> -f|--file <Vault root key configaration json file>`

There are also optional parameters, see `CustodyCli.exe initvaultdb --help` for all options.

Example: `CustodyCli.exe initvaultrootkey -u http://localhost:5000/ -f vault-root-key-config.json`

```json
{
  "tenantName": "general",
  "threshold": 2,
  "rootKeyHolders": [
    {
      "id": "9c970324e7c98d37dbc215666ae6c8c4336756177b208c6c2d7d2f58fae17dee",
      "name": "RKH 1"
    },
    {
      "id": "3dd412b6b2e6f9326fadecb11de960f31e942acd537fcb7b5f21be6204804043",
      "name": "RKH 2"
    },
    {
      "id": "eb421f2366dfec7c71333c00f1c5b777a0f9bd7e7ece26c5403723b0b4f6559a",
      "name": "RKH 5"
    }
  ]
}
```

### Vault root key rotation

Initializes new Vault root key parameters. Sends root key configuration to the Vault that starts the rotation process.

`CustodyCli.exe rotatevaultrootkey -u|--url <Vault URL> -f|--file <Vault root key configaration json file>`

There are also optional parameters, see `CustodyCli.exe initvaultdb --help` for all options.

Example: `CustodyCli.exe rotatevaultrootkey -u http://localhost:5000/ -f vault-root-key-config.json`

```json
{
  "tenantName": "general",
  "threshold": 2,
  "rootKeyHolders": [
    {
      "id": "9c970324e7c98d37dbc215666ae6c8c4336756177b208c6c2d7d2f58fae17dee",
      "name": "RKH 3"
    },
    {
      "id": "eb421f2366dfec7c71333c00f1c5b777a0f9bd7e7ece26c5403723b0b4f6559a",
      "name": "RKH 5"
    }
  ]
}
```