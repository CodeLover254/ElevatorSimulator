using ElevatorSimulator.Domain.Enums;

namespace ElevatorSimulator.Domain.Models;

public abstract class BaseElevator
{
    public string Label { get; set; }
    public int CurrentFloor { get; set; } = 0;
    public ElevatorType ElevatorType { get; set; }
    public Direction Direction { get; set; } = Direction.NEUTRAL;
    public ElevatorState ElevatorState { get; set; } = ElevatorState.Idle;
    protected PriorityQueue<Request, int> RequestQueue { get; set; } = new PriorityQueue<Request, int>();

    public abstract bool IsFullyLoaded();
    public abstract int RemainingCapacity();
    
    public abstract string GetStatus();

    public int GetAvailabilityScore(Request request)
    {
        //Check for matching request type
        if (request.RequestType != ElevatorType) return (int)ElevatorAvailability.Unavailable;
        var score = 0;
        //closeness to the requester
        var distance = Math.Abs(CurrentFloor - request.SourceFloor);
        score += distance == 0 ? int.MaxValue : 1 / distance;
        //direction as a factor
        if (request.Direction == Direction)
        {
            score += 2;
        }else if (Direction == Direction.NEUTRAL)
        {
            score += 1;
        }

        return score;
    }
    
}