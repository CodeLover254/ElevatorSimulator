using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Domain.Comparers;

public class RequestInverseComparer: IComparer<Request>
{
    public int Compare(Request? x, Request? y)
    {
        if (x.DestinationFloor == y.DestinationFloor) return 0;
        if (x.DestinationFloor < y.DestinationFloor) return 1;
        return -1;
    }
}