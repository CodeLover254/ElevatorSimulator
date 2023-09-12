using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorControlService
{
    IEnumerable<string> GetElevatorLabels();
    string GetElevatorStatus(string elevatorLabel);
    IEnumerable<string> GetAllElevatorStatus();
    Task ActivateElevatorsAsync();
    Task<EnqueueResult> EnqueueRequestAsync(Request request);
}