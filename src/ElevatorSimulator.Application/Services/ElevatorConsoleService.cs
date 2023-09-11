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

    private void CreateSetup()
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
        _elevatorControlService.ActivateElevators();
    }

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

            if (!predicate.Invoke(input))
            {
                Console.WriteLine(message);
                continue;
            }
            break;
        }

        return input;
    }
    
    private async Task CreateMenu()
    {
        while (true)
        {
            Console.WriteLine(@"
        ===================================
               Elevator System
        ===================================
        1. View Single Elevator Status
        2. View All Elevators Status
        3. Request Elevator
        4. Quit Application
        ===================================
        ");
            int option = GetAndValidateNumericInput(input=> input < 1 || input > 4, "Invalid input. Try again");
            
            if(option == 4) break;

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
    }


    private void ProcessViewSingleElevatorStatus()
    {
        var elevatorLabels = _elevatorControlService.GetElevatorLabels();
        Console.WriteLine($"Select an elevator: [{string.Join(',',elevatorLabels)}]");
        string elevatorLabel;
        while (true)
        {
            elevatorLabel = Console.ReadLine();
            if (string.IsNullOrEmpty(elevatorLabel) || !elevatorLabel.Contains(elevatorLabel))
            {
                Console.WriteLine("Please enter a valid elevator label");
                continue;
            }
            break;
        }

        Console.WriteLine(_elevatorControlService.GetElevatorStatus(elevatorLabel));
        Console.WriteLine("");
    }

    private void ProcessViewAllElevatorStatus()
    {
        var statuses = _elevatorControlService.GetAllElevatorStatus();
        foreach (var status in statuses)
        {
            Console.WriteLine(status);
        }
        Console.WriteLine("");
    }

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
        var selectedElevator = await _elevatorControlService.EnqueueRequest(new Request
        {
            SourceFloor = sourceFloor,
            DestinationFloor = destinationFloor,
            Capacity = passengers,
            Direction = sourceFloor < destinationFloor ? Direction.Up : Direction.Down,
            RequestType = ElevatorType.Passenger
        });
        
        Console.WriteLine($"Elevator {selectedElevator.Label} is on its way.");
    }

    public async Task Interact()
    {
        try
        {
            CreateSetup();
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