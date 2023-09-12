using ElevatorSimulator.Application.Services;
using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Exceptions;
using ElevatorSimulator.Domain.Models;
using FluentAssertions;

namespace ElevatorSimulator.UnitTests
{
    public class ElevatorControlServiceTests
    {
        private readonly ElevatorControlService _testSubject;
        private readonly PassengerElevator _passengerElevatorSubject;
        
        public ElevatorControlServiceTests()
        {
            var floors = 20;
            var elevatorCapacity = 10;
            _passengerElevatorSubject = new PassengerElevator
            {
                Label = "P1",
                CurrentFloor = 0,
                HighestFloor = floors - 1,
                LowestFloor = 0,
                MaximumPassengers = elevatorCapacity,
                ElevatorType = ElevatorType.Passenger
            };
            
            var elevators = new Dictionary<string, BaseElevator>
            {
                {
                    "P1", _passengerElevatorSubject
                },
                {
                    "P2", new PassengerElevator
                    {
                        Label = "P2",
                        CurrentFloor = 6,
                        HighestFloor = floors-1,
                        LowestFloor = 0,
                        MaximumPassengers = elevatorCapacity,
                        ElevatorType = ElevatorType.Passenger
                    }
                }, 
                {
                    "P3", new PassengerElevator
                    {
                        Label = "P3",
                        CurrentFloor = 0,
                        HighestFloor = floors-1,
                        LowestFloor = 0,
                        MaximumPassengers = elevatorCapacity,
                        ElevatorType = ElevatorType.Passenger
                    }
                },
                {
                    "F1", new FreightElevator()
                    {
                        Label = "F1",
                        CurrentFloor = 0,
                        HighestFloor = floors-1,
                        LowestFloor = 0,
                        MaximumWeight = elevatorCapacity,
                        ElevatorType = ElevatorType.Freight
                    }
                }
            };
            _testSubject = new ElevatorControlService(new Building
            {
                Elevators = elevators,
                NumFloors = floors,
                NumElevators = elevators.Count
            });
        }

        [Fact]
        public void GetElevatorLabels_ReturnsCorrectNumberOfLabels()
        {
            var result = _testSubject.GetElevatorLabels().ToList();
            result.Should().HaveCount(4);
            result.Should().ContainInOrder(new List<string> { "P1", "P2", "P3", "F1" });
        }
        
        [Fact]
        public void GetElevatorStatus_ReturnsCorrectStatusForSingleElevator()
        {
            var result = _testSubject.GetElevatorStatus(_passengerElevatorSubject.Label);
            var expected =
                $"Elevator {_passengerElevatorSubject.Label} is Idle at Floor {_passengerElevatorSubject.CurrentFloor} carrying {_passengerElevatorSubject.Passengers} passengers";
            result.Should().Be(expected);
        }
        
        [Fact]
        public void GetElevatorStatus_ThrowsIfSuppliedWrongLabel()
        {
            Action action = () => _testSubject.GetElevatorStatus("W1");
            action.Should().Throw<DomainException>();
        }
        
        [Fact]
        public void GetAllElevatorStatus_ReturnsStatusForAllElevators()
        {
            var result = _testSubject.GetAllElevatorStatus().ToList();
            result.Should().HaveCount(4);
        }

        [Fact]
        public void ActivateElevatorsAsync_ThrowsNoExceptions()
        {
            Func<Task> activation = async () => await _testSubject.ActivateElevatorsAsync();
            activation.Should().NotThrowAsync();
        }

        [Fact]
        public async Task EnqueueRequestAsync_ReturnsCorrectlyScheduledElevator()
        {
            var result = await _testSubject.EnqueueRequestAsync(new Request
            {
                Capacity = 5,
                SourceFloor = 4,
                DestinationFloor = 18,
                RequestType = ElevatorType.Passenger,
            });

            result.ElevatorLabel.Should().Be("P2");
            result.BoardedCapacity.Should().Be(5);

        }
    }
}