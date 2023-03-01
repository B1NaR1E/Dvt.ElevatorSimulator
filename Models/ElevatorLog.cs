using Dvt.ElevatorSimulator.Enums;

namespace Dvt.ElevatorSimulator.Models;

public class ElevatorLog
{
    public ElevatorLog(Guid elevatorId, LogType type, int currentFloor, int currentDestination,
        Direction currentDirection, int totalPassengers, string? message = null)
    {
        ElevatorId = elevatorId;
        CurrentFloor = currentFloor;
        CurrentDestination = currentDestination;
        CurrentDirection = currentDirection;
        TotalPassengers = totalPassengers;
        Type = type;
        Created = DateTime.Now;
        Message = message;
    }
    
    public Guid ElevatorId { get; set; }
    public int CurrentFloor { get; set; }
    public int CurrentDestination { get; set; }
    public int TotalPassengers { get; set; }
    public string? Message { get; set; }
    public Direction CurrentDirection { get; set; }
    public LogType Type { get; set; }
    public DateTime Created { get; set; }
}