using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Sirius.Tools.CustodyCLI.Clients;
using Sirius.Tools.CustodyCLI.Commands;

namespace Sirius.Tools.CustodyCLI
{
    public class Program
    {
        static void Main(string[] args)
        {
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
                    .AddSingleton<CustodyApiClient>();

                var serviceProvider = serviceCollection.BuildServiceProvider();

                var application = new CommandLineApplication(false)
                {
                    Name = "Sirius.Tools.Custody",
                    Description = ".NET Core console app for working with custody",
                };
                application.HelpOption("-?|-h|--help");
                application.VersionOption("-v|--version",
                    () => $"Version {Assembly.GetEntryAssembly()?.GetName().Version}");

                var commandFactory = new CommandFactory(serviceProvider);

                var generateCommandRegistration = new GenerateCommandRegistration(commandFactory);
                var encryptCommandRegistration = new EncryptCommandRegistration(commandFactory);
                var setupCommandRegistration = new SetupCommandRegistration(commandFactory);

                application.Command("generate", generateCommandRegistration.StartExecution, false);
                application.Command("encrypt", encryptCommandRegistration.StartExecution, false);
                application.Command("setup", setupCommandRegistration.StartExecution, false);

                ErrorRedirect.RedirectErrorToStandardError(() => application.Execute(args));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
