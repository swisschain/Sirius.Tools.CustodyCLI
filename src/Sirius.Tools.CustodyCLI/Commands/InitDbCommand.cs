using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class InitDbCommand : ICommand
    {
        private string _connectionString;
        private string _dbName;
        private string _groupName;
        private string _userName;
        private string _userPassword;
        private int _connectionsLimit;
        private int _dbConnectionsLimit;
        private readonly ILogger<InitDbCommand> _logger;

        public InitDbCommand(string connectionString, 
            string dbName, 
            string groupName, 
            string userName, 
            string userPassword, 
            int connectionsLimit, 
            int dbConnectionsLimit, 
            ILogger<InitDbCommand> logger)
        {
            _connectionString = connectionString;
            _dbName = dbName;
            _groupName = groupName;
            _userName = userName;
            _userPassword = userPassword;
            _connectionsLimit = connectionsLimit;
            _dbConnectionsLimit = dbConnectionsLimit;
            _logger = logger;
        }

        public async Task<int> ExecuteAsync()
        {
            using var connection = new Npgsql.NpgsqlConnection(_connectionString);

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            _logger.LogInformation("Ensuring group role {groupRole} and user role {userRole} exist...", _groupName, _userName);

            await CreateRoles(connection);

            _logger.LogInformation("Setting user {userRole} password...", _userName);

            await SetUserPassword(connection);

            if (await DoesDbExist(connection))
            {
                _logger.LogInformation("DB {dbName} already exists skipping its creation", _dbName);
            }
            else
            {
                _logger.LogInformation("Creating DB {dbName}...", _dbName);

                await CreateDb(connection);

                _logger.LogInformation("Updating DB {dbName} connections limit to {dbConnectionsLimit}...", _dbName, _dbConnectionsLimit);

                await UpdateDbConnectionsLimit(connection);
            }            

            return 0;
        }

        private async Task CreateRoles(Npgsql.NpgsqlConnection connection)
        {
            var rolesCommand = connection.CreateCommand();

            rolesCommand.CommandText = $@"
DO
$do$
BEGIN
    IF EXISTS (
        SELECT FROM pg_catalog.pg_roles
        WHERE rolname = '{_groupName}') THEN

        RAISE NOTICE 'Role ""{_groupName}"" already exists. Skipping.';
    ELSE      
        CREATE ROLE {_groupName} WITH
            NOLOGIN
            NOSUPERUSER
            INHERIT
            NOCREATEDB
            NOCREATEROLE
            NOREPLICATION;
    END IF;
END
$do$;

DO
$do$
BEGIN
    IF EXISTS (
        SELECT FROM pg_catalog.pg_roles
        WHERE rolname = '{_userName}') THEN

        RAISE NOTICE 'Role ""{_userName}"" already exists. Skipping.';
    ELSE     
        CREATE ROLE {_userName} WITH
            LOGIN
            NOSUPERUSER
            INHERIT
            NOCREATEDB
            NOCREATEROLE
            NOREPLICATION;

        ALTER ROLE {_userName} CONNECTION LIMIT {_connectionsLimit};
    END IF;
END
$do$;

GRANT {_groupName} TO {_userName};
GRANT {_groupName} TO {connection.UserName};
";

            await rolesCommand.ExecuteNonQueryAsync();
        }

        private async Task SetUserPassword(Npgsql.NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();

            command.CommandText = $"ALTER USER {_userName} WITH PASSWORD '{_userPassword}';";

            await command.ExecuteNonQueryAsync();
        }

        private async Task<bool> DoesDbExist(Npgsql.NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();
            
            command.CommandText = $"SELECT EXISTS (SELECT FROM pg_catalog.pg_database WHERE datname = '{_dbName}')";

            return (bool)await command.ExecuteScalarAsync();
        }

        private async Task CreateDb(Npgsql.NpgsqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = @$"
CREATE DATABASE {_dbName}
    WITH 
    OWNER = {_groupName}
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;
";

            await command.ExecuteNonQueryAsync();
        }

        private async Task UpdateDbConnectionsLimit(Npgsql.NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();

            command.CommandText = $"ALTER DATABASE {_dbName} WITH CONNECTION LIMIT = {_dbConnectionsLimit};";

            await command.ExecuteNonQueryAsync();
        }
    }
}
