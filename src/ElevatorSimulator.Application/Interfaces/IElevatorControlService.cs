using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorControlService
{
    IEnumerable<string> GetElevatorLabels();
    string GetElevatorStatus(string elevatorLabel);
    IEnumerable<string> GetAllElevatorStatus();
    void ActivateElevators();
    Task<BaseElevator> EnqueueRequest(Request request);
}