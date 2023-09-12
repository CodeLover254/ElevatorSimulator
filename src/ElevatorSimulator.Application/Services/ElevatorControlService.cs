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

    public IEnumerable<string> GetElevatorLabels()
    {
        return _building.Elevators.Select(e => e.Value.Label);
    }

    public string GetElevatorStatus(string elevatorLabel)
    {
        var elevator = _building.Elevators[elevatorLabel];
        if (elevator == null) throw new DomainException("Invalid elevator label");
        return elevator.GetStatus();
    }

    public IEnumerable<string> GetAllElevatorStatus()
    {
        if (!_building.Elevators.Any()) throw new DomainException("No available elevators in the building");
        return _building.Elevators.Select(e => e.Value.GetStatus());
    }

    public void ActivateElevators()
    {
        //todo make this async and parallel
        foreach (var elevator in _building.Elevators.Values)
        {
            elevator.Activate();
        }
    }


    public async Task<EnqueueResult> EnqueueRequest(Request request)
    {
        return await Task.Run(() =>
        {
            var availableElevators = _building.Elevators.Select((kvPair) =>
                {
                    var elevator = kvPair.Value;
                    return (elevator.Label, AvailabilityScore: elevator.GetAvailabilityScore(request));
                })
                .OrderByDescending(x => x.AvailabilityScore)
                .ToList();

            if (!availableElevators.Any())
                throw new DomainException("No available elevators to serve your request");

            var highestScoringElevator = availableElevators.First();
            var selectedElevator = _building.Elevators[highestScoringElevator.Label];
            var boarded = selectedElevator.ScheduleRequest(request);


            return new EnqueueResult
            {
                ElevatorLabel = selectedElevator.Label,
                BoardedCapacity = boarded
            };
        });
    }
}