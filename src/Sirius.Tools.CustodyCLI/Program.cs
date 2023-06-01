using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuardianApiClient;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Sirius.Tools.CustodyCLI.Commands;
using VaultApiClient;

try
{
    var serviceCollection = new ServiceCollection();

    serviceCollection
        .AddLogging(logging =>
        {
            logging.AddSerilog(
                new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.RollingFile("logs/{Date}-custody-cli.log")
                    .CreateLogger());
            logging.AddSerilog(
                new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console(LogEventLevel.Information, "{Message:lj}{NewLine}")
                    .CreateLogger());
        })
        .AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        })
        .AddHttpClient()
        .AddSingleton<CustodyApiClient>()
        .AddSingleton<IVaultApiClientFactory, VaultApiClientFactory>();

    var serviceProvider = serviceCollection.BuildServiceProvider();

    var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
    jsonOptions.Converters.Add(new JsonStringEnumConverter());


    var application = new CommandLineApplication(false)
    {
        Name = "Sirius.Tools.Custody", Description = ".NET Core console app for working with custody",
    };
    application.HelpOption("-?|-h|--help");
    application.VersionOption("-v|--version",
        () => $"Version {Assembly.GetEntryAssembly()?.GetName().Version}");

    var commandFactory = new CommandFactory(serviceProvider);

    var generateCommandRegistration = new GenerateCommandRegistration(commandFactory);
    var encryptCommandRegistration = new EncryptCommandRegistration(commandFactory);
    var setupCommandRegistration = new SetupCommandRegistration(commandFactory);
    var initGuardianDbCommandRegistration = new InitGuardianDbCommandRegistration(commandFactory);
    var initVaultDbCommandRegistration = new InitVaultDbCommandRegistration(commandFactory);
    var signWithdrawalRequestV1Registration = new SignWithdrawalV1CommandRegistration(commandFactory);
    var signWithdrawalRequestV2Registration = new SignWithdrawalV2CommandRegistration(commandFactory);
    var initVaultRootKeyCommandRegistration = new InitVaultRootKeyCommandRegistration(commandFactory, jsonOptions);
    var rotateVaultRootKeyCommandRegistration = new RotateVaultRootKeyCommandRegistration(commandFactory, jsonOptions);

    application.Command("generate", generateCommandRegistration.StartExecution, false);
    application.Command("encrypt", encryptCommandRegistration.StartExecution, false);
    application.Command("setup", setupCommandRegistration.StartExecution, false);
    application.Command("initguardiandb", initGuardianDbCommandRegistration.StartExecution, false);
    application.Command("initvaultdb", initVaultDbCommandRegistration.StartExecution, false);
    application.Command("signwithdrawal", signWithdrawalRequestV2Registration.StartExecution, false);
    application.Command("initvaultrootkey", initVaultRootKeyCommandRegistration.StartExecution, false);
    application.Command("rotatevaultrootkey", rotateVaultRootKeyCommandRegistration.StartExecution, false);

    ErrorRedirect.RedirectErrorToStandardError(() => application.Execute(args));
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
