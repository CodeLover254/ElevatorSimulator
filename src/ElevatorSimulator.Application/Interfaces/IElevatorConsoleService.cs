using ElevatorSimulator.Domain.Settings;
using Microsoft.Extensions.Options;

namespace ElevatorSimulator.Application.Interfaces;

public interface IElevatorConsoleService
{
   Task Interact();
}