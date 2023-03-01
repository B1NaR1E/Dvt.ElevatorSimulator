using Dvt.ElevatorSimulator;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

//Console.WriteLine("Please enter total number of floors:");
var numberOfFloors = 10; //Convert.ToInt32(Console.ReadLine());

//Console.WriteLine("Please enter total number of elevators:");
var numberOfElevators = 10; //Convert.ToInt32(Console.ReadLine());

//Console.WriteLine("Please enter total number of requests:");
var numberOfRequests = 200; //Convert.ToInt32(Console.ReadLine());

var maxumumPassengers = 5;

var pickupCount = 0;
var stepCount = 0;
var random = new Random();
var passengerLimit = 5;

List<Elevator> elevators = Enumerable.Range(0, numberOfElevators)
    .Select(eid => 
        new Elevator(new ElevatorPassengerManager(maxumumPassengers), new ElevatorLogManager()))
    .ToList();

IElevatorControlSystem system = new ElevatorControlSystem(elevators);


while (pickupCount < numberOfRequests)
{
    var originatingFloor = random.Next(1, numberOfFloors + 1);
    var destinationFloor = random.Next(1, numberOfFloors + 1);
    if (originatingFloor != destinationFloor)
    {
        var totalPassengers = random.Next(1, maxumumPassengers + 1);
        system.Pickup(originatingFloor, destinationFloor, totalPassengers);
        pickupCount++;
    }
}

Console.Clear();
Console.Write("WELCOME TO THE ELEVATOR SIMULATOR\n-------------------------------------------\n\nPlease select one of the following options:\n\n1. Start Simulation.\n2. Exit\n\n-->");

var input = Convert.ToInt32(Console.ReadLine());
if (input == 1)
{
    Console.Clear();
    Console.Write($"SIMULATOR RUNNING\n---------------------------------\nTotal Elevators: {numberOfElevators}\nTotal Floors: {numberOfFloors}\nTotal Passengers: {numberOfRequests}\n");
    Console.Write("Processing:");
    while (system.AnyOutstandingPickups())
    {
        system.Step();
        stepCount++;
        Console.Write("*");
    }
    Console.Write($"\nDONE!\n\nTransported {numberOfRequests} passengers to their requested destinations in {stepCount} steps.\n");
    
    Console.Write("Please select one of the following options:\n\n1. View Elevator Reports.\n2. Exit\n\n-->");
    input = Convert.ToInt32(Console.ReadLine());
    if (input == 1)
    {
        Console.Clear();
        foreach (var elevator in elevators)
        {
            Console.WriteLine($"ELEVATOR {elevator.Id} REPORT");
            Console.WriteLine("-------------------------------");

            foreach (var elevatorLog in elevator.GetElevatorLogs())
            {
                Console.WriteLine($"[{elevatorLog.Created}]: {elevatorLog.Message} CurrentFloor: {elevatorLog.CurrentFloor}, Destination: {elevatorLog.CurrentDestination}, TotalPassengers: {elevatorLog.TotalPassengers}");
            }
            Console.WriteLine("\n");
        }
        
        Console.WriteLine("Please select one of the following options:\n\n1. Exit\n\n-->");
    }
}