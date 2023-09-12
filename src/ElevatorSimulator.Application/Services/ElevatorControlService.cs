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

    /// <summary>
    /// Method returns all elevator labels
    /// </summary>
    /// <returns>An enumerable containing all elevator labels</returns>
    public IEnumerable<string> GetElevatorLabels()
    {
        return _building.Elevators.Select(e => e.Value.Label);
    }

    /// <summary>
    /// Method returns elevator status showing the elevator label,
    /// current status, floor and loading for a single elevator
    /// </summary>
    /// <param name="elevatorLabel"></param>
    /// <returns>A string containing the elevator status info</returns>
    /// <exception cref="DomainException"></exception>
    public string GetElevatorStatus(string elevatorLabel)
    {
        var elevator = _building.Elevators[elevatorLabel];
        if (elevator == null) throw new DomainException("Invalid elevator label");
        return elevator.GetStatus();
    }

    /// <summary>
    /// Method returns elevator status showing the elevator label,
    /// current status, floor and loading for all elevators
    /// </summary>
    /// <returns>The list of elevator status</returns>
    /// <exception cref="DomainException"></exception>
    public IEnumerable<string> GetAllElevatorStatus()
    {
        if (!_building.Elevators.Any()) throw new DomainException("No available elevators in the building");
        return _building.Elevators.Select(e => e.Value.GetStatus());
    }

    /// <summary>
    /// Method activates all elevators in the building asynchronously
    /// </summary>
    public async Task ActivateElevatorsAsync()
    {
        var activationTasks = _building.Elevators.Values.Select(x => x.ActivateAsync());
        await Task.WhenAll(activationTasks);
    }

    /// <summary>
    /// Method selects the most available elevator based on their availability
    /// score and sends a request for it to be scheduled
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public async Task<EnqueueResult> EnqueueRequestAsync(Request request)
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