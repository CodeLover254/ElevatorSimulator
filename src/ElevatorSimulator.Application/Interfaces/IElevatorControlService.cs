using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorControlService
{
    string GetElevatorStatus(string elevatorLabel);
    IEnumerable<string> GetAllElevatorStatus();
    Task<BaseElevator> EnqueueRequest(Request request);
}