using Dvt.ElevatorSimulator.Enums;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevator<TPassenger> where TPassenger : IPassenger
{
    Guid Id { get; }
    int CurrentFloor { get; }
    int DestinationFloor { get; }
    Direction Direction { get; }
    State State { get; }
    bool HasPassengers { get; }
    bool HasCapacity { get; }
    List<int> Stops { get; }
    bool LoadPassenger(List<TPassenger> passenger);
    int UnloadPassengers();
    void AddStop(int destinationFloor);
    void Move();
    int TotalPassengers();
}