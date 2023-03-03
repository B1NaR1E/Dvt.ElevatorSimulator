using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IPassengerManager<in TPassenger> 
    where TPassenger : Passenger
{
    bool HasPassengers { get; }
    bool OverPassengerLimit { get; }
    void LoadPassenger(TPassenger passenger);
    int UnloadPassengers(int floorNumber);
    int UnloadOverLimitPassengers(int floorNumber);
    int TotalPassenger();
}