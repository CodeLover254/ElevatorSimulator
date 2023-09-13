namespace ElevatorSimulator.Domain.Settings;

public class BuildingSettings
{
    public const string Name = "BuildingSettings";
    public int Floors { get; set; }
    public List<ElevatorSettings> Elevators { get; set; }
}