namespace Dvt.ElevatorSimulator.Models;

public class PassengerWithWeight : Passenger
{
    public decimal Weight { get; }

    public PassengerWithWeight(int originatingFloor, int destinationFloor, decimal weight) : base(originatingFloor, destinationFloor)
    {
        Weight = weight;
    }
}