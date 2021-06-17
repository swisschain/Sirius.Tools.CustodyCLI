using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
                                .WriteTo.File("logs/custody-cli.log")
                                .CreateLogger());
                    })
                    .AddSingleton(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    })
                    .AddHttpClient();

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