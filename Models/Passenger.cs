using Dvt.ElevatorSimulator.Enums;

namespace Dvt.ElevatorSimulator.Models;

public class Passenger
{
    public Passenger(int originatingFloor, int destinationFloor)
    {
        Id = Guid.NewGuid();
        OriginatingFloor = originatingFloor;
        DestinationFloor = destinationFloor;
    }

    public Guid Id { get; }
    public int OriginatingFloor { get;}
    public int DestinationFloor { get; }
    
    public Direction Direction => OriginatingFloor < DestinationFloor ? Direction.Up : Direction.Down;
}