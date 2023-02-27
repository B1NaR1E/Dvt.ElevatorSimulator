using Dvt.ElevatorSimulator.Enums;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevator
{
    int Id { get; }
    int CurrentFloor { get; set; }
    int DestinationFloor { get; set; }
    Direction Direction { get; }
    State State { get; }
}