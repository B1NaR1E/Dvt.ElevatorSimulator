using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IPassengerManager<TPassenger> 
    where TPassenger : Passenger
{
    bool HasPassengers { get; }
    bool OverPassengerLimit { get; }
    void LoadPassenger(TPassenger passenger);
    IEnumerable<TPassenger> UnloadPassengers(int floorNumber);
    IEnumerable<TPassenger> UnloadOverLimitPassengers(int floorNumber);
    int TotalPassenger();
}