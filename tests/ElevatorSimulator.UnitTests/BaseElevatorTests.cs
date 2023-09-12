using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.UnitTests;

public class BaseElevatorTests
{
    private readonly BaseElevator _testSubject1;
    private readonly BaseElevator _testSubject2;
    private readonly BaseElevator _testSubject3;
    private readonly Building _building;
    public BaseElevatorTests()
    {
        var floors = 20;
        var capacity = 10;
        _testSubject1 = new PassengerElevator
        {
            Label = "P1",
            CurrentFloor = 0,
            HighestFloor = floors - 1,
            LowestFloor = 0,
            MaximumPassengers = capacity,
            ElevatorType = ElevatorType.Passenger,
            Passengers = 5
        };
        
        _testSubject2 = new PassengerElevator
        {
            Label = "P2",
            CurrentFloor = 9,
            HighestFloor = floors - 1,
            LowestFloor = 0,
            MaximumPassengers = capacity,
            ElevatorType = ElevatorType.Passenger,
        };
        
        _testSubject2 = new FreightElevator()
        {
            Label = "P2",
            CurrentFloor = 9,
            HighestFloor = floors - 1,
            LowestFloor = 0,
            MaximumWeight= capacity,
            ElevatorType = ElevatorType.Passenger,
            CurrentWeight = capacity
        };

        var elevatorStore = new Dictionary<string, BaseElevator>();
        elevatorStore[_testSubject1.Label] = _testSubject1;
        elevatorStore[_testSubject2.Label] = _testSubject2;
        elevatorStore[_testSubject3.Label] = _testSubject3;

        _building = new Building
        {
            NumFloors = floors,
            NumElevators = 3,
            Elevators = elevatorStore
        };
    }

    [Fact]
    public void IsFullyLoaded_ReturnsCorrectState()
    {
        
    }
    
    [Fact]
    public void RemainingCapacity_ReturnsCorrectValue()
    {
        
    }
    
    [Fact]
    public void GetMaximumCapacity_ReturnsCorrectValue()
    {
        
    }

    [Fact]
    public void GetStatus_ReturnsCorrectElevatorStatus()
    {
        
    }

    [Fact]
    private void ModifyLoading_CorrectlyMutatesElevatorLoad()
    {
        
    }

    [Fact]
    private void GetAvailabilityScore_ReturnsUnavailableForFullyLoadedElevator()
    {
        
    }

    [Fact]
    public void DetermineDefaultDirection_ReturnsCorrectDirection()
    {
        
    }

    [Fact]
    public void ScheduleRequest_ReturnsCorrectBoardingNumber()
    {
        
    }
    
    [Fact]
    public void Run_SuccessfullyRunsElevatorProcesses()
    {
        
    }
}