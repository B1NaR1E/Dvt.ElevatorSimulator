using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Elevator : ElevatorBase<Passenger>
{
    private readonly int _totalFloors;

    public Elevator(IPassengerManager<Passenger> passengerManager, int totalFloors) 
        : base(passengerManager)
    {
        _totalFloors = totalFloors;
    }

    protected override int GetDestination()
    {
        return Stops.Any() ? Stops.MinBy(s => Math.Abs(s - CurrentFloor)) : DestinationFloor;
    }

    protected override bool LoadPassenger(List<Passenger> passengers)
    {
        foreach (var passenger in passengers)
        {
            PassengerManager.LoadPassenger(passenger);
        }

        var destinationFloor = passengers.First().DestinationFloor;

        if (PassengerManager.OverPassengerLimit)
        {
            State = State.OverLimit;
            return false;
        }

        AddStop(destinationFloor);
        return true;
    }

    protected override List<Passenger> UnloadPassengers()
    {
        if (!PassengerManager.OverPassengerLimit) 
            return PassengerManager.UnloadPassengers(CurrentFloor);
        
        State = State.Stopped;
        return PassengerManager.UnloadOverLimitPassengers(CurrentFloor);
    }

    protected override void MoveUp()
    {
        if (CurrentFloor != _totalFloors)
        {
            ++CurrentFloor;
        }
    }

    protected override void MoveDown()
    {
        if (CurrentFloor != 1)
        {
            --CurrentFloor;
        }
    }
}