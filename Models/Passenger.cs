using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Passenger : IPassenger
{
    public Passenger(int originatingFloor, int destinationFloor)
    {
        OriginatingFloor = originatingFloor;
        DestinationFloor = destinationFloor;
    }
    
    public int OriginatingFloor { get;}
    public int DestinationFloor { get; }
}