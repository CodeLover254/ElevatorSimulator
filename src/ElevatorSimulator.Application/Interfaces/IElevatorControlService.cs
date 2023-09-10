using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorControlService
{
    Task<string> GetElevatorStatus(string elevatorLabel);
    Task<IEnumerable<string>> GetAllElevatorStatus();
    Task<BaseElevator> EnqueueRequest(Request request);
}