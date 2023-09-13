using ElevatorSimulator.Domain.Enums;

namespace ElevatorSimulator.Domain.Models;

public class Request
{
    public int SourceFloor { get; set; }
    public int DestinationFloor { get; set; }
    public int Capacity { get; set; }
    public Direction Direction { get; set; }
    public ElevatorType RequestType { get; set; }
}