using Dvt.ElevatorSimulator.Enums;

namespace Dvt.ElevatorSimulator.Models;

public class ElevatorCallRequest
{
    public ElevatorCallRequest(int originatingFloor, int destinationFloor, int totalPassengers)
    {
        OriginatingFloor = originatingFloor;
        DestinationFloor = destinationFloor;
        TotalPassengers = totalPassengers;
    }

    public int OriginatingFloor { get; }
    public int DestinationFloor { get; }
    public int TotalPassengers { get; }
    public Direction Direction => OriginatingFloor < DestinationFloor ? Direction.Up : Direction.Down;
}