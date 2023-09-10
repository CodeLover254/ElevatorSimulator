namespace ElevatorSimulator.Domain.Models;

public class PassengerElevator: BaseElevator
{
    public int MaximumPassengers { get; set; }
    public int Passengers { get; set; }

    public override bool IsFullyLoaded()
    {
        return Passengers == MaximumPassengers;
    }

    public override int RemainingCapacity()
    {
        return MaximumPassengers - Passengers;
    }

    public override string GetStatus()
    {
        throw new NotImplementedException();
    }
}