using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevator
{
    Guid Id { get; }
    int CurrentFloor { get; }
    int DestinationFloor { get; }
    Direction Direction { get; }
    State State { get; }
    bool HasPassengers { get; }
    bool HasCapacity { get; }
    List<int> Stops { get; }
    bool LoadPassenger(int totalPassengers, int destinationFloor);
    int UnloadPassengers();
    void AddStop(int destinationFloor);
    void Move();
    int TotalPassengers();
}