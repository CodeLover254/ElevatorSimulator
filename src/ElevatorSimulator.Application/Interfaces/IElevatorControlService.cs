using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorControlService
{
    IEnumerable<string> GetElevatorLabels();
    string GetElevatorStatus(string elevatorLabel);
    IEnumerable<string> GetAllElevatorStatus();
    void ActivateElevators();
    Task<EnqueueResult> EnqueueRequest(Request request);
}