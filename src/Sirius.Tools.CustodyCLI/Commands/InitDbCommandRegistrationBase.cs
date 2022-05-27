using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sirius.Tools.CustodyCLI.Commands
{
    public class InitDbCommandRegistrationBase : ICommandRegistration
    {
        private readonly CommandFactory _factory;
        private readonly string _description;
        private readonly string _defaultUserName;

        public InitDbCommandRegistrationBase(CommandFactory factory,
            string description,
            string defaultUserName)
        {
            _factory = factory;
            _description = description;
            _defaultUserName = defaultUserName;
        }

        public void StartExecution(CommandLineApplication lineApplication)
        {
            lineApplication.Description = _description;
            lineApplication.HelpOption("-?|-h|--help");

            var connectionStringOption = lineApplication.Option(
                "-c|--conn <string>",
                "Database server connection string. See https://www.npgsql.org/doc/connection-string-parameters.html",
                CommandOptionType.SingleValue);

            var dbNameOption = lineApplication.Option(
                "-d|--dbname <string>",
                "Custody DB name to create or to use. Default is `custody`",
                CommandOptionType.SingleValue);

            var groupNameOption = lineApplication.Option(
                "-g|--group <string>",
                "Custody users group name to create or to use. Default is `sirius_users_group`",
                CommandOptionType.SingleValue);

            var userNameOption = lineApplication.Option(
                "-n|--uname <string>",
                $"Custody user name to create or to update. Default is `{_defaultUserName}`",
                CommandOptionType.SingleValue);

            var userPasswordOption = lineApplication.Option(
                "-p|--upass <string>",
                "Custody user password to specify",
                CommandOptionType.SingleValue);

            var connectionsLimitOption = lineApplication.Option(
                "--connlimit <integer>",
                "Custody user connections limit to specify. Default is `50`",
                CommandOptionType.SingleValue);

            var dbConnectionsLimitOption = lineApplication.Option(
                "--dbconnlimit <integer>",
                "Custody DB connections limit to specify. Default is `-1`",
                CommandOptionType.SingleValue);

            lineApplication.OnExecute(async () =>
            {
                var connectionString = connectionStringOption.Value();
                var dbName = dbNameOption.Value() ?? "custody";
                var groupName = groupNameOption.Value() ?? "sirius_users_group";
                var userName = userNameOption.Value() ?? _defaultUserName;
                var userPassword = userPasswordOption.Value();
                var connectionsLimitString = connectionsLimitOption.Value() ?? "50";
                var dbConnectionsLimitString = dbConnectionsLimitOption.Value() ?? "-1";

                if (string.IsNullOrEmpty(connectionString))
                    throw new OptionInvalidException("Database server connection string should be specified.");

                if (string.IsNullOrEmpty(dbName))
                    throw new OptionInvalidException("Custody database name should be specified. Take away the option to use default value.");

                if (string.IsNullOrEmpty(groupName))
                    throw new OptionInvalidException("Custody group name should be specified. Take away the option to use default value.");

                if (string.IsNullOrEmpty(userName))
                    throw new OptionInvalidException("Custody user name should be specified. Take away the option to use default value.");

                static bool IsPasswordStrong(string pwd)
                {
                    return !string.IsNullOrEmpty(pwd) &&
                        pwd.Length >= 16 &&
                        pwd.Any(c => char.IsUpper(c)) &&
                        pwd.Any(c => char.IsLower(c)) &&
                        pwd.Any(c => char.IsDigit(c));
                }

                if (!IsPasswordStrong(userPassword))
                    throw new OptionInvalidException("Custody user password should be specified and should be strong enough - minimum 16 characters, should include lower case letters, upper case letters digits.");

                if (string.IsNullOrEmpty(connectionsLimitString) || !int.TryParse(connectionsLimitString, out var connectionsLimit))
                    throw new OptionInvalidException(
                        "Custody user connections limit should be specified and it should be an integer number. Take away the option to use default value.");

                if (string.IsNullOrEmpty(dbConnectionsLimitString) || !int.TryParse(dbConnectionsLimitString, out var dbConnectionsLimit))
                    throw new OptionInvalidException(
                        "Custody DB connections limit should be specified and it should be an integer number. Take away the option to use default value.");

                var command = _factory.CreateCommand(serviceProvider =>
                    new InitDbCommand(
                        connectionString,
                        dbName,
                        groupName,
                        userName,
                        userPassword,
                        connectionsLimit,
                        dbConnectionsLimit,
                        serviceProvider.GetRequiredService<ILogger<InitDbCommand>>()));

                return await command.ExecuteAsync();
            });
        }
    }
}
