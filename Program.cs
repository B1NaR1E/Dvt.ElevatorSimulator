using Dvt.ElevatorSimulator;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;
using Dvt.ElevatorSimulator.Strategies;

IElevatorControlSystem system;
List<IElevator<IPassenger>> elevators;

string? errorMessage = null;

int steps = 0;

var numberOfFloors = GetTotalFloorsDisplay();

var numberOfElevators = GetTotalElevatorsDisplay();

var maximumPassengers = GetTotalPassengersInput();

EmulatorSetup(numberOfElevators, numberOfFloors, maximumPassengers);

Console.Clear();
int result;

do
{
    result = GetRunEmulatorDisplay();

    switch (result)
    {
        case 1:
            while (system.AnyOutstandingPickups())
            {
                system.Step();
                Console.Clear();
                DisplayElevatorStatus();
                DisplayRequestInformation();
                ++steps;
                Thread.Sleep(1000);
            }
            break;
        case 2:
            system.Step();
            ++steps;
            break;
        case 3:
            AddRequestManually();
            break;
        case 4:
            var numberOfRequests = GetTotalRequestsInput();
            GenerateRequests(numberOfRequests);
            steps = 0;
            break;
    }
}
while (result != 5);

void DisplayRequestInformation()
{
    Console.WriteLine($"Total Elevator Calls Remaining: {system.GetTotalRequests()}.");
    Console.WriteLine($"Total Passengers Remaining: {system.GetTotalPassengers()}.");
    Console.WriteLine($"Total Steps Taken: {steps}.");
    Console.WriteLine();
}

int GetRunEmulatorDisplay()
{
    while (true)
    {
        Console.Clear();
        
        DisplayElevatorStatus();
        DisplayRequestInformation();

        List<int> validInputs = new List<int>()
        {
            1,2,3,4,5
        };

        if (!string.IsNullOrEmpty(errorMessage)) 
            Console.WriteLine(errorMessage);

        Console.Write("Please select one of the following options:\n1. Run Emulator\n2. Step Emulator\n3. Add Request\n4. Generate Requests\n5. Exit\n-->");

        bool successful = int.TryParse(Console.ReadLine(), out var input);

        if (!successful || !validInputs.Contains(input))
        {
            errorMessage = "ERROR: Invalid input. Please enter a valid input.";
            continue;
        }

        errorMessage = null;

        return input;
    }
}

void DisplayElevatorStatus()
{
    Console.WriteLine("Elevators");
    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
    var elevatorNumber = 1;
    
    var elevatorNumbers = "";
    var elevatorState = "";
    var elevatorDirection = "";
    var currentFloor = "";
    var totalPassengers = "";
    var destinationFloor = "";

    var numberOfRows = elevators.Count / 5;

    numberOfRows = elevators.Count % 5 > 0 ? numberOfRows + 1 : numberOfRows;

    for (var i = 0; i < numberOfRows; i++)
    {
        var elevatorsToPrint = elevators.Skip(i * 5).Take(5);

        foreach (var elevator in elevatorsToPrint)
        {
            elevatorNumbers += $"No: {elevatorNumber}\t\t\t";
            elevatorState += elevator.State == Dvt.ElevatorSimulator.Enums.State.OverLimit ? $"Status: {elevator.State}\t" : $"Status: {elevator.State}\t\t";
            elevatorDirection += elevator.Direction == Dvt.ElevatorSimulator.Enums.Direction.Static ? $"Direction: {elevator.Direction}\t" : $"Direction: {elevator.Direction}\t\t";
            currentFloor += $"Current Floor: {elevator.CurrentFloor}\t";
            destinationFloor += $"Destination Floor: {elevator.DestinationFloor}\t";
            totalPassengers += $"Total Passengers: {elevator.TotalPassengers()}\t";
            ++elevatorNumber;
        }

        Console.WriteLine(elevatorNumbers);
        Console.WriteLine(elevatorState);
        Console.WriteLine(elevatorDirection);
        Console.WriteLine(currentFloor);
        Console.WriteLine(destinationFloor);
        Console.WriteLine(totalPassengers);
        Console.WriteLine();
        
        elevatorNumbers = "";
        elevatorState = "";
        elevatorDirection = "";
        currentFloor = "";
        destinationFloor = "";
        totalPassengers = "";
    }

    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
}

void EmulatorSetup(int totalElevators, int totalFloors, int maxPassengers)
{
    elevators = Enumerable.Range(0, totalElevators)
        .Select(_ =>
            new Elevator(new ElevatorPassengerManager(maxPassengers), totalFloors) as IElevator<IPassenger>)
        .ToList();

    IStrategy<IPassenger> schedulingStrategy = new SchedulingStrategy<IPassenger>();
    system = new ElevatorControlSystem<IPassenger>(schedulingStrategy, elevators);
}

int GetTotalElevatorsDisplay()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage)) 
            Console.WriteLine(errorMessage);

        Console.Write("Please enter the total number of elevators:\n-->");

        var successful = int.TryParse(Console.ReadLine(), out var totalElevators);

        if (!successful)
        {
            errorMessage = "ERROR: Invalid input. Please enter a valid input for total elevators.";
            continue;
        }

        errorMessage = null;

        return totalElevators;
    }
}

int GetTotalFloorsDisplay()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage)) 
            Console.WriteLine(errorMessage);

        Console.Write($"Please enter the total number of floors:\n-->");

        var successful = int.TryParse(Console.ReadLine(), out var totalFloors);

        if (!successful)
        {
            errorMessage = "ERROR: Invalid input. Please enter a valid input for total floors.";
            continue;
        }

        errorMessage = null;

        return totalFloors;
    }
}

int GetTotalRequestsInput()
{
    while (true)
    {
        Console.Clear();
        DisplayElevatorStatus();
        DisplayRequestInformation();

        if (!string.IsNullOrEmpty(errorMessage)) 
            Console.WriteLine(errorMessage);

        Console.Write("Please enter the total number of requests to create:\n-->");

        var successful = int.TryParse(Console.ReadLine(), out var totalRequests);

        if (!successful)
        {
            errorMessage = "ERROR: Invalid input. Please enter a valid input for total requests.";
            continue;
        }

        errorMessage = null;

        return totalRequests;
    }
}

int GetTotalPassengersInput()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine();

        if (!string.IsNullOrEmpty(errorMessage)) 
            Console.WriteLine(errorMessage);

        Console.Write("Please enter the maximum number of passengers per elevator(Elevator passenger limit):\n-->");

        var successful = int.TryParse(Console.ReadLine(), out var totalPassengers);

        if (!successful)
        {
            errorMessage = "ERROR: Invalid input. Please enter a valid input for total passengers.";
            continue;
        }

        errorMessage = null;

        return totalPassengers;
    }
}

void GenerateRequests(int totalRequests)
{
    var random = new Random();
    var pickupCount = 0;
    while (pickupCount < totalRequests)
    {
        var originatingFloor = random.Next(1, numberOfFloors + 1);
        var destinationFloor = random.Next(1, numberOfFloors + 1);
        if (originatingFloor == destinationFloor) continue;
        var totalPassengers = random.Next(1, maximumPassengers + 1);
        system.Pickup(originatingFloor, destinationFloor, totalPassengers);
        pickupCount++;
    }
}

void AddRequestManually()
{
    int currentFloor, destinationFloor, totalPassengers;
    string? message = null;
    bool successful;
    do
    {
        Console.Clear();
        
        DisplayElevatorStatus();
        DisplayRequestInformation();
        
        Console.WriteLine();
        
        Console.Write($"Creating a elevator call request.\n{message}\nPlease enter current floor number:\n-->");
        successful = int.TryParse(Console.ReadLine(), out currentFloor);

        if(!successful)
            message = "ERROR: Invalid input. Please enter a valid input.";

        if(currentFloor > numberOfFloors)
        {
            message = $"ERROR: Current floor cannot be greater than the top floor {numberOfFloors}.";
            successful = false;
        }

        if (currentFloor >= 1) 
            continue;
        
        message = $"ERROR: Current floor cannot be smaller than the ground floor {1}.";
        successful = false;
    }
    while (!successful);
    
    message = null;
    
    do
    {
        Console.Clear();
        
        DisplayElevatorStatus();
        DisplayRequestInformation();
        
        Console.WriteLine();
        
        Console.Write($"Creating a elevator call request.\n{message}\nPlease enter destination floor number:\nCurrent Floor: {currentFloor}.\n-->");
        successful = int.TryParse(Console.ReadLine(), out destinationFloor);

        if (!successful)
            message = "ERROR: Invalid input. Please enter a valid input.";

        if (destinationFloor > numberOfFloors)
        {
            message = $"ERROR: Destination floor cannot be greater than the top floor {numberOfFloors}.";
            successful = false;
        }

        if (destinationFloor < 1)
        {
            message = $"ERROR: Destination floor cannot be smaller than the ground floor {1}.";
            successful = false;
        }

        if (destinationFloor != currentFloor) 
            continue;
        
        message = $"ERROR: Destination floor cannot be the same as the current floor.";
        successful = false;
    }
    while (!successful);
    
    message = null;
    
    do
    {
        Console.Clear();
        
        DisplayElevatorStatus();
        DisplayRequestInformation();
        
        Console.WriteLine();
        
        Console.Write($"Creating a elevator call request.\n{message}\nPlease enter number of passengers:\nCurrent Floor: {currentFloor}.\nDestination Floor: {destinationFloor}\n-->");
        successful = int.TryParse(Console.ReadLine(), out totalPassengers);

        if (!successful)
            message = "ERROR: Invalid input. Please enter a valid input.";
        
        if (totalPassengers < 1)
        {
            message = $"ERROR: Total passengers cannot be smaller than 1";
            successful = false;
        }

        if (totalPassengers <= maximumPassengers) 
            continue;
        
        message = $"ERROR: Total passengers cannot be greater than the maximum allowed passengers: {maximumPassengers}";
        successful = false;
    }
    while (!successful);
    
    system.Pickup(currentFloor, destinationFloor, totalPassengers);
}
