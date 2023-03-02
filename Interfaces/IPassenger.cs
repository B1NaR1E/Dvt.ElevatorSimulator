namespace Dvt.ElevatorSimulator.Interfaces;

public interface IPassenger
{
    int OriginatingFloor { get; }
    int DestinationFloor { get; }
}