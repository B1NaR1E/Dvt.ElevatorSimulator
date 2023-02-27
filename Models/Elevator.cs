using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Elevator : IElevator
{
    public int Id { get; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public Direction Direction =>
        CurrentFloor == 1
            ? Direction.Up
            : DestinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
    public State State { get; }
    public List<Passenger> Passengers { get; set; }

    public Elevator(int id)
    {
        Id = id;
        Passengers = new List<Passenger>();
        State = State.Idle;
    }
}