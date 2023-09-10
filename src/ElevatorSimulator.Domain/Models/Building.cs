namespace ElevatorSimulator.Domain.Models;

public class Building
{
    public int NumFloors { get; set; }
    public int NumElevators { get; set; }
    public Dictionary<string, BaseElevator> Elevators { get; set; } = new ();
}