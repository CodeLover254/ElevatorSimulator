using ElevatorSimulator.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

//configure application configuration
var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//configure logging
var loggingFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var configuration = configurationBuilder.Build();
var consoleService = new ElevatorConsoleService(configuration, loggingFactory);
consoleService.Interact();


//needed for XUnit tests
public partial class Program{}