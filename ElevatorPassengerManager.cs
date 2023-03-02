using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator;

public class ElevatorPassengerManager : IPassengerManager<IPassenger>
{
    private readonly int _maximum;
    private readonly List<IPassenger> _passengers;
    public bool HasCapacity => TotalPassenger() < _maximum;
    public bool HasPassengers => _passengers.Count > 0;
    public bool OverPassengerLimit => TotalPassenger() > _maximum;

    public ElevatorPassengerManager(int maximum)
    {
        _maximum = maximum;
        _passengers = new List<IPassenger>();
    }

    public void LoadPassenger(IPassenger passenger)
    {
        _passengers.Add(passenger);
    }

    public int UnloadPassengers(int floorNumber)
    {
        return _passengers.RemoveAll(p => p.DestinationFloor == floorNumber);
    }

    public int TotalPassenger()
    {
        return _passengers.Count;
    }

    public int UnloadOverLimitPassengers(int floorNumber)
    {
        return _passengers.RemoveAll(p => p.OriginatingFloor == floorNumber);
    }
}