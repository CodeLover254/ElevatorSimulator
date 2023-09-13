using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Models;
using FluentAssertions;

namespace ElevatorSimulator.UnitTests;

public class BaseElevatorTests
{
    private readonly BaseElevator _testSubject1;
    private readonly BaseElevator _testSubject2;
    private readonly BaseElevator _testSubject3;
    private const int Floors = 20;
    private const int Capacity = 10;
    private const int Passengers = 5;
    public BaseElevatorTests()
    {
        
        _testSubject1 = new PassengerElevator
        {
            Label = "P1",
            CurrentFloor = 0,
            HighestFloor = Floors - 1,
            LowestFloor = 0,
            MaximumPassengers = Capacity,
            Passengers = Passengers
        };
        
        _testSubject2 = new PassengerElevator
        {
            Label = "P2",
            CurrentFloor = 9,
            HighestFloor = Floors - 1,
            LowestFloor = 0,
            MaximumPassengers = Capacity,
        };
        
        _testSubject3 = new FreightElevator()
        {
            Label = "P2",
            CurrentFloor = 9,
            HighestFloor = Floors - 1,
            LowestFloor = 0,
            MaximumWeight= Capacity,
            CurrentWeight = Capacity
        };
    }

    [Fact]
    public void IsFullyLoaded_ReturnsCorrectState()
    {
        var result1 = _testSubject1.IsFullyLoaded();
        var result2 = _testSubject3.IsFullyLoaded();

        result1.Should().BeFalse();
        result2.Should().BeTrue();
    }
    
    [Fact]
    public void RemainingCapacity_ReturnsCorrectValue()
    {
        var expectedResult = Capacity - Passengers;
        var result = _testSubject1.RemainingCapacity();
        result.Should().Be(expectedResult);
    }
    
    [Fact]
    public void GetMaximumCapacity_ReturnsCorrectValue()
    {
        var result = _testSubject2.GetMaximumCapacity();
        result.Should().Be(Capacity);
    }

    [Fact]
    private void ModifyLoading_CorrectlyMutatesElevatorLoad()
    {
        var additionalPassengers = 3;
        var totalPassegers = Passengers + additionalPassengers;
        var expectedResult = Capacity - totalPassegers;
        _testSubject1.ModifyLoading(additionalPassengers, ElevatorLoadingOptions.Add);
        var result = _testSubject1.RemainingCapacity();
        result.Should().Be(expectedResult);
    }

    [Fact]
    private void GetAvailabilityScore_ReturnsUnavailableForFullyLoadedElevator()
    {
        var result = _testSubject3.GetAvailabilityScore(new Request
        {
            RequestType = ElevatorType.Freight,
            Capacity = 10
        });

        result.Should().Be((int)ElevatorAvailability.Unavailable);
    }
    
    [Fact]
    private void GetAvailabilityScore_ReturnsUnavailableForFullyUnrelatedRequestType()
    {
        var result = _testSubject1.GetAvailabilityScore(new Request
        {
            RequestType = ElevatorType.Freight
        });

        result.Should().Be((int)ElevatorAvailability.Unavailable);
    }

    [Fact]
    public void ScheduleRequest_ReturnsCorrectBoardingNumber()
    {
        var boardingPassengers = 3;
        var expectedResult = Math.Min(_testSubject1.RemainingCapacity(),boardingPassengers);
        var result = _testSubject1.ScheduleRequest(new Request
        {
            Capacity = boardingPassengers,
            DestinationFloor = 10,
            RequestType = ElevatorType.Passenger,
            SourceFloor = 0
        });

        result.Should().Be(expectedResult);
    }
    
    [Fact]
    public async Task Run_SuccessfullyRunsElevatorProcesses()
    {
        var expectedResult = 12;
        
        _testSubject2.ScheduleRequest(new Request
        {
            Capacity = 2,
            DestinationFloor = expectedResult,
            RequestType = ElevatorType.Passenger,
            SourceFloor = 9
        });

        await _testSubject2.ActivateAsync();
        
        Thread.Sleep(10000); //sleep to allow elevator to arrive.

        var currentFloor = _testSubject2.CurrentFloor;
        currentFloor.Should().Be(expectedResult);
    }
}