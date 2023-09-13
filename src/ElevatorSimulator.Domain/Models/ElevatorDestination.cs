using ElevatorSimulator.Domain.Enums;

namespace ElevatorSimulator.Domain.Models;

public class ElevatorDestination
{
    public int FloorNumber { get; set; }
    public ElevatorDestinationType DestinationType { get; set; }
    public int Capacity { get; set; }
}