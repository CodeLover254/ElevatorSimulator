using ElevatorSimulator.Application.Interfaces;
using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Application.Services;

public class ElevatorControlService: IElevatorControlService
{
    private readonly Building _building;

    public ElevatorControlService(Building building)
    {
        _building = building;
    }

    public Task<string> GetElevatorStatus(string elevatorLabel)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetAllElevatorStatus()
    {
        throw new NotImplementedException();
    }

    public Task<BaseElevator> EnqueueRequest(Request request)
    {
        throw new NotImplementedException();
    }
}