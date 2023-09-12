using ElevatorSimulator.Application.Interfaces;
using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Exceptions;
using ElevatorSimulator.Domain.Models;
using ElevatorSimulator.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElevatorSimulator.Application.Services;

public class ElevatorConsoleService: IElevatorConsoleService
{
    private readonly BuildingSettings? _buildingSettings;
    private IElevatorControlService _elevatorControlService;
    private readonly ILogger<ElevatorConsoleService> _logger;

    public ElevatorConsoleService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _buildingSettings = configuration.GetSection(BuildingSettings.Name).Get<BuildingSettings>();;
        _logger = loggerFactory.CreateLogger<ElevatorConsoleService>();
    }

    /// <summary>
    /// Method sets up application configuration and configures
    /// the building with floors and elevators
    /// </summary>
    /// <exception cref="DomainException"></exception>
    private async Task CreateSetup()
    {
        var elevators = new Dictionary<string, BaseElevator>();

        if (_buildingSettings == null) throw new DomainException("No configuration found for application setup");
        if (_buildingSettings.Floors < 1) throw new DomainException("Invalid floor numbers. Setup failed.");
        foreach (var elevatorSettings in _buildingSettings.Elevators)
        {
            var elevatorType = Enum.Parse<ElevatorType>(elevatorSettings.Type);
            if (elevatorType == null) throw new DomainException("Invalid elevator type. Setup failed.");
            if (elevatorSettings.Number < 0 || elevatorSettings.Capacity < 1)
                throw new DomainException("Invalid number of elevators or capacity. Setup failed");
            for (int i = 0; i < elevatorSettings.Number; i++)
            {
                BaseElevator elevator;
                if (elevatorType == ElevatorType.Passenger)
                {
                    elevator = new PassengerElevator
                    {
                        ElevatorType = ElevatorType.Passenger,
                        HighestFloor = _buildingSettings.Floors - 1,
                        LowestFloor = 0,
                        Label = $"P{i + 1}",
                        MaximumPassengers = elevatorSettings.Capacity
                    };
                }
                else
                {
                    elevator = new FreightElevator()
                    {
                        ElevatorType = ElevatorType.Freight,
                        HighestFloor = _buildingSettings.Floors - 1,
                        LowestFloor = 0,
                        Label = $"F{i + 1}",
                        MaximumWeight = elevatorSettings.Capacity
                    };
                }

                elevators[elevator.Label] = elevator;
            }
        }

        var building = new Building
        {
            NumFloors = _buildingSettings.Floors,
            Elevators = elevators,
            NumElevators = elevators.Count
        };

        _elevatorControlService = new ElevatorControlService(building);
       await _elevatorControlService.ActivateElevatorsAsync();
    }

    /// <summary>
    /// Method takes standard input from console, validates
    /// if the input is a valid numeric then runs the input
    /// through a predicate for more validation.
    /// </summary>
    /// <param name="predicate">Predicate</param>
    /// <param name="message">string</param>
    /// <returns></returns>
    private int GetAndValidateNumericInput(Predicate<int> predicate,string message)
    {
        int input;
        while (true)
        {
            bool valid = int.TryParse(Console.ReadLine(), out input);
            if (!valid)
            {
                Console.WriteLine("Enter a valid number");
                continue;
            }

            if (predicate(input))
            {
                Console.WriteLine(message);
                continue;
            }
            break;
        }

        return input;
    }
    
    /// <summary>
    /// Method creates a menu and takes care of the application interaction loop
    /// </summary>
    private async Task CreateMenu()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("================================");
                Console.WriteLine("         Elevator System        ");
                Console.WriteLine("=================================");
                Console.WriteLine("1. View Single Elevator Status");
                Console.WriteLine("2. View All Elevators Status");
                Console.WriteLine("3. Request Elevator");
                Console.WriteLine("4. Quit Application");
                Console.WriteLine("=================================");
                int option = GetAndValidateNumericInput(input => input < 1 || input > 4, "Invalid input. Try again");

                if (option == 4) break;

                switch (option)
                {
                    case 1:
                        ProcessViewSingleElevatorStatus();
                        break;
                    case 2:
                        ProcessViewAllElevatorStatus();
                        break;
                    case 3:
                        await ProcessRequestElevator();
                        break;
                }
            }
            catch (DomainException domainException)
            {
                Console.WriteLine(domainException.Message);
            }
        }
    }


    /// <summary>
    /// Method presents single elevator status to the user
    /// </summary>
    private void ProcessViewSingleElevatorStatus()
    {
        var elevatorLabels = _elevatorControlService.GetElevatorLabels();
        Console.WriteLine($"Select an elevator: [{string.Join(',',elevatorLabels)}]");
        string elevatorLabel;
        while (true)
        {
            elevatorLabel = Console.ReadLine();
            if (string.IsNullOrEmpty(elevatorLabel) || !elevatorLabels.Contains(elevatorLabel))
            {
                Console.WriteLine("Please enter a valid elevator label");
                continue;
            }
            break;
        }

        Console.WriteLine(_elevatorControlService.GetElevatorStatus(elevatorLabel));
        Console.WriteLine("");
    }

    /// <summary>
    /// Method presents all elevator status to the user
    /// </summary>
    private void ProcessViewAllElevatorStatus()
    {
        var statuses = _elevatorControlService.GetAllElevatorStatus();
        foreach (var status in statuses)
        {
            Console.WriteLine(status);
        }
        Console.WriteLine("");
    }

    /// <summary>
    /// Method creates session for user to request an elevator
    /// </summary>
    private async Task ProcessRequestElevator()
    {
        Console.WriteLine("Enter your floor: ");
        int sourceFloor =
            GetAndValidateNumericInput(i => i < 0 || i >= _buildingSettings!.Floors, "Invalid floor number input.Try again");
        Console.WriteLine("Enter destination floor: ");
        int destinationFloor = GetAndValidateNumericInput(i => i < 0 || i == sourceFloor || i >= _buildingSettings!.Floors,
            "Invalid destination floor.Try again.");
        Console.WriteLine("Enter number of passengers: ");
        int passengers = GetAndValidateNumericInput(i => i < 0, "There must be at least one passenger");
        var enqueueRequest = await _elevatorControlService.EnqueueRequestAsync(new Request
        {
            SourceFloor = sourceFloor,
            DestinationFloor = destinationFloor,
            Capacity = passengers,
            Direction = sourceFloor < destinationFloor ? Direction.Up : Direction.Down,
            RequestType = ElevatorType.Passenger
        });
        
        Console.WriteLine($"Elevator {enqueueRequest.ElevatorLabel} is on its way. Only {enqueueRequest.BoardedCapacity} will be taken.");
    }

    /// <summary>
    /// Method calls CreateSetup and CreateMenu  to complete
    /// the application startup
    /// </summary>
    public async Task Interact()
    {
        try
        {
            await CreateSetup();
            await CreateMenu();
        }
        catch (DomainException domainException)
        {
            _logger.LogWarning(domainException.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"Something went wrong");
        }
    }
}