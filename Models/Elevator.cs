using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Elevator : ElevatorBase<IPassenger>
{
    private readonly int _totalFloors;

    public Elevator(IPassengerManager<IPassenger> passengerManager, int totalFloors) 
        : base(passengerManager)
    {
        _totalFloors = totalFloors;
    }

    protected override int GetDestination()
    {
        return Stops.Any() ? Stops.MinBy(s => Math.Abs(s - CurrentFloor)) : DestinationFloor;
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