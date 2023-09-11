using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Domain.Comparers;

public class RequestComparer: IComparer<Request>
{
    public int Compare(Request x, Request y)
    {
        return x.DestinationFloor.CompareTo(y.DestinationFloor);
    }
}