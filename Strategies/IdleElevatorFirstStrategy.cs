using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Strategies;

public class IdleElevatorFirstStrategy: IStrategy<Passenger>
{
    public Guid ProcessRequest(IEnumerable<IElevator<Passenger>> elevators, ElevatorCallRequest request)
    {
        IElevator<Passenger>? selectedElevator = null;
        
        var staticElevators = elevators.Where(e => 
                e.Direction == Direction.Static && 
                e.State != State.OverLimit)
            .ToList();

        if (staticElevators.Any())
            return GetClosestElevator(staticElevators, request.OriginatingFloor).Id;

        switch (request.Direction)
        {
            case Direction.Up:
            {
                var elevatorsGoingUp = elevators.Where(e =>
                        e.Direction == request.Direction &&
                        e.CurrentFloor <= request.OriginatingFloor &&
                        e.State != State.OverLimit)
                    .ToList();

                if (elevatorsGoingUp.Any())
                    selectedElevator = GetClosestElevator(elevatorsGoingUp, request.OriginatingFloor);
                break;
            }
            case Direction.Down:
            {
                var elevatorsGoingDown = elevators.Where(e =>
                        e.Direction == request.Direction &&
                        e.CurrentFloor >= request.OriginatingFloor &&
                        e.State != State.OverLimit)
                    .ToList();

                if (elevatorsGoingDown.Any())
                    selectedElevator = GetClosestElevator(elevatorsGoingDown, request.OriginatingFloor);
                break;
            }
        }

        return selectedElevator?.Id ?? Guid.Empty;
    }

    private static IElevator<Passenger> GetClosestElevator(IEnumerable<IElevator<Passenger>> elevators, int originatingFloor)
    {
        return elevators.Aggregate((x, y) =>
            Math.Abs(x.CurrentFloor - originatingFloor) <
            Math.Abs(y.CurrentFloor - originatingFloor) ? x : y);
    }
}