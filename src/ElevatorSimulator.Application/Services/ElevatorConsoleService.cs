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
    }


    public void Interact()
    {
        try
        {
            CreateSetup();
            
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