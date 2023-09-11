using ElevatorSimulator.Application.Interfaces;
using ElevatorSimulator.Domain.Exceptions;
using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Services;

public class ElevatorControlService: IElevatorControlService
{
    private readonly Building _building;

    public ElevatorControlService(Building building)
    {
        _building = building;
    }

    public string GetElevatorStatus(string elevatorLabel)
    {
        var elevator = _building.Elevators[elevatorLabel];
        if (elevator == null) throw new ElevatorControlException("Invalid elevator label");
        return elevator.GetStatus();
    }

    public IEnumerable<string> GetAllElevatorStatus()
    {
        if (!_building.Elevators.Any()) throw new ElevatorControlException("No available elevators in the building");
        return _building.Elevators.Select(e => e.Value.GetStatus());
    }

    public Task<BaseElevator> EnqueueRequest(Request request)
    {
        throw new NotImplementedException();
    }
}