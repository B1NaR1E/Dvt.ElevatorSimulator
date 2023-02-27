using Dvt.ElevatorSimulator;
using Dvt.ElevatorSimulator.Interfaces;

Console.WriteLine("Please enter total number of floors:");
var numberOfFloors = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("Please enter total number of elevators:");
var numberOfElevators = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("Please enter total number of requests:");
var numberOfRequests = Convert.ToInt32(Console.ReadLine());

var pickupCount = 0;
var stepCount = 0;
var random = new Random();
IElevatorControlSystem system = new ElevatorControlSystem(numberOfElevators);


while (pickupCount < numberOfRequests)
{
    var originatingFloor = random.Next(1, numberOfFloors + 1);
    var destinationFloor = random.Next(1, numberOfFloors + 1);
    if (originatingFloor != destinationFloor)
    {
        system.Pickup(originatingFloor, destinationFloor);
        pickupCount++;
    }
}

while (system.AnyOutstandingPickups())
{
    system.Step();
    stepCount++;
}

Console.WriteLine("Transported {0} elevator riders to their requested destinations in {1} steps.", pickupCount, stepCount);
Console.ReadLine();