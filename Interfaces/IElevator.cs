using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevator<TPassenger> where TPassenger : Passenger
{
    Guid Id { get; }
    int CurrentFloor { get; }
    int DestinationFloor { get; }
    Direction Direction { get; }
    State State { get; }
    bool HasPassengers { get; }
    List<int> Stops { get; }
    bool LoadPassenger(List<TPassenger> passenger);
    List<TPassenger> UnloadPassengers();
    void AddStop(int destinationFloor);
    void Move();
    int TotalPassengers();
}