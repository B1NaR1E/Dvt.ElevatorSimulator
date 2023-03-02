namespace Dvt.ElevatorSimulator.Interfaces;

public interface IPassengerManager<in TPassenger> 
    where TPassenger : IPassenger
{
    bool HasCapacity { get; }
    bool HasPassengers { get; }
    bool OverPassengerLimit { get; }
    void LoadPassenger(TPassenger passenger);
    int UnloadPassengers(int floorNumber);
    int UnloadOverLimitPassengers(int floorNumber);
    int TotalPassenger();
}