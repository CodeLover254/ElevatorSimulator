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
        return Task.Run(() =>
        {
            var availableElevators = _building.Elevators.Select((kvPair) =>
                {
                    var elevator = kvPair.Value;
                    return (elevator.Label, AvailabilityScore: elevator.GetAvailabilityScore(request));
                })
                .OrderByDescending(x => x.AvailabilityScore)
                .ToList();

            if (!availableElevators.Any())
                throw new ElevatorControlException("No available elevators to serve your request");

            var highestScoringElevator = availableElevators.First();
            var selectedElevator = _building.Elevators[highestScoringElevator.Label];
            var assignResult = selectedElevator.ReceiveRequest(request);
            if (!assignResult) throw new ElevatorControlException("Unable to request elevator. Try again");

            return selectedElevator;
        });
    }
}