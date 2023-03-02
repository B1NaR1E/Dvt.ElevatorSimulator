using Dvt.ElevatorSimulator;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;


IElevatorControlSystem system;
List<Elevator> elevators;

var stepCount = 0;
string errorMessage = null;

var numberOfFloors = GetTotalFloorsDisplay();

var numberOfElevators = GetTotalElevatorsDisplay();

var maxumumPassengers = GetTotalPassengersInput();

var numberOfRequests = 0;

EmulatorSetup(numberOfElevators, numberOfFloors, maxumumPassengers, false);

Console.Clear();
int input = 0;

do
{
    input = GetRunEmulatorDisplay();

    switch (input)
    {
        case 1:
            while (system.AnyOutstandingPickups())
            {
                system.Step();
                ++stepCount;
                Console.Clear();
                DisplayElevatorStatus();
                DisplayRequestInformation();
                Thread.Sleep(2000);
            }
            break;
        case 2:
            system.Step();
            ++stepCount;
            break;
        case 3:
            AddRequestManually();
            break;
        case 4:
            numberOfRequests = GetTotalRequestsInput();
            GenerateRequests(numberOfRequests);
            break;
        default:
            break;
    }
}
while (input != 5);


void DisplayRequestInformation()
{
    Console.WriteLine($"Total Elevator Calls Remaining: {system.GetTotalRequests()}.");
    Console.WriteLine($"Total Passengers Remaining: {system.GetTotalPassengers()}.");
}

int GetRunEmulatorDisplay()
{
    Console.Clear();
    DisplayElevatorStatus();
    DisplayRequestInformation();

    int input = 0;
    List<int> validInputs = new List<int>() { 1, 2, 3, 4, 5 };

    if (string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Please select one of the following options:\n1. Run Emulator\n2. Emualate Step\n3. Add Request\n4. Generate Requests\n5. Exit\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out input);

    if (!successful || !validInputs.Contains(input))
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input.";
        return GetRunEmulatorDisplay();
    }
    errorMessage = null;

    return input;
}

void DisplayAllElevatorsReports()
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

void DisplayElevatorStatus()
{
    Console.WriteLine("Elevators");
    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
    int elevatorNumber = 1;

    string elevatorIds = "";
    string numbers = "";
    string states = "";
    string direction = "";
    string currentFloor = "";
    string totalPassengers = "";
    string destinationFloor = "";

    int numberOfRows = elevators.Count / 5;

    numberOfRows = elevators.Count % 5 > 0 ? numberOfRows + 1 : numberOfRows;

    for (int i = 0; i < numberOfRows; i++)
    {
        var elevatorsToPrint = elevators.Skip(i * 5).Take(5);

        foreach (var elevator in elevatorsToPrint)
        {
            elevatorIds += $"ElevatorID: {elevator.Id}\t\t\t";
            numbers += $"No: {elevatorNumber}\t\t\t";
            states += elevator.State == Dvt.ElevatorSimulator.Enums.State.OverLimit ? $"Status: {elevator.State}\t" : $"Status: {elevator.State}\t\t";
            direction += elevator.Direction == Dvt.ElevatorSimulator.Enums.Direction.Static ? $"Direction: {elevator.Direction}\t" : $"Direction: {elevator.Direction}\t\t";
            currentFloor += $"Current Floor: {elevator.CurrentFloor}\t";
            destinationFloor += $"Destination Floor: {elevator.DestinationFloor}\t";
            totalPassengers += $"Total Passengers: {elevator.TotalPassengers()}\t";
            ++elevatorNumber;
        }

        Console.WriteLine(numbers);
        Console.WriteLine(states);
        Console.WriteLine(direction);
        Console.WriteLine(currentFloor);
        Console.WriteLine(destinationFloor);
        Console.WriteLine(totalPassengers);
        Console.WriteLine();

        elevatorIds = "";
        numbers = "";
        states = "";
        direction = "";
        currentFloor = "";
        destinationFloor = "";
        totalPassengers = "";
    }

    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
}

void EmulatorSetup(int totalElevators, int totalFloors, int maximumPassengers, bool overrideLimit)
{
    //IPassengerManager passengerManager = new ElevatorPassengerManager(maximumPassengers);
    //IElevatorLogManager<ElevatorLog> elevatorLogManager = new ElevatorLogManager();
    elevators = Enumerable.Range(0, numberOfElevators)
    .Select(eid =>
        new Elevator(new ElevatorPassengerManager(maximumPassengers), new ElevatorLogManager()))
    .ToList();

    system = new ElevatorControlSystem(elevators);
}

int GetTotalElevatorsDisplay()
{
    Console.Clear();
    int totalElevators = 0;


    if (string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Please enter the total number of elevators:\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out totalElevators);

    if (!successful)
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input for total elevators.";
        return GetTotalElevatorsDisplay();
    }
    errorMessage = null;

    return totalElevators;
}


int GetTotalFloorsDisplay()
{
    Console.Clear();
    int totalFloors = 0;

    if (string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Please enter the total number of floors:\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out totalFloors);

    if (!successful)
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input for total floors.";
        return GetTotalFloorsDisplay();
    }
    errorMessage = null;

    return totalFloors;
}

int GetTotalRequestsInput()
{
    Console.Clear();
    DisplayElevatorStatus();
    DisplayRequestInformation();
    int totalRequests = 0;

    if (string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Please enter the total number of requests to create:\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out totalRequests);

    if (!successful)
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input for total requests.";
        return GetTotalRequestsInput();
    }
    errorMessage = null;

    return totalRequests;
}

int GetGenerateRequestsDisplay()
{
    int input = 0;
    List<int> validInputs = new List<int>() { 1, 2, 3 };

    if (string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Do you want to generate requests or add them manually:\n1. Generate Requests\n2. Add Request Manually\n3. Exit\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out input);

    if (!successful || !validInputs.Contains(input))
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input.";
        return GetGenerateRequestsDisplay();
    }
    errorMessage = null;

    return input;
}

int GetTotalPassengersInput()
{
    Console.Clear();
    int totalPassengers = 0;

    if(string.IsNullOrEmpty(errorMessage))
        Console.WriteLine(errorMessage);

    Console.Write("Please enter the maximum number of passengers per elevator:\n-->");

    bool successful = int.TryParse(Console.ReadLine(), out totalPassengers);

    if (!successful)
    {
        errorMessage = "ERROR: Invalid imput. Please enter a valid input for total passengers.";
        return GetTotalPassengersInput();
    }
    errorMessage = null;

    return totalPassengers;
}


void GenerateRequests(int totalRequests)
{
    var random = new Random();
    int pickupCount = 0;
    while (pickupCount < totalRequests)
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
}

void AddRequestManually()
{
    int currentFloor, destinationFloor, totalPassengers;
    string message = null;
    bool successfull = false;
    do
    {
        Console.Clear();
        DisplayElevatorStatus();
        DisplayRequestInformation();
        Console.WriteLine();
        Console.Write($"Createing a elevator call request.\n{message}\nPlease enter current floor number:\n-->");
        successfull = int.TryParse(Console.ReadLine(), out currentFloor);

        if(!successfull)
            message = "ERROR: Invalid imput. Please enter a valid input.";

        if(currentFloor > numberOfFloors)
        {
            message = $"ERROR: Current floor cannot be greater than the top floor {numberOfFloors}.";
            successfull = false;
        }

        if (currentFloor < 1)
        {
            message = $"ERROR: Current floor cannot be smaller than the ground floor {1}.";
            successfull = false;
        }
    }
    while (!successfull);

    do
    {
        Console.Clear();
        DisplayElevatorStatus();
        DisplayRequestInformation();
        Console.WriteLine();
        Console.Write($"Createing a elevator call request.\n{message}\nPlease enter destination floor number:\nCurrent Floor: {currentFloor}.\n-->");
        successfull = int.TryParse(Console.ReadLine(), out destinationFloor);

        if (!successfull)
            message = "ERROR: Invalid imput. Please enter a valid input.";

        if (destinationFloor > numberOfFloors)
        {
            message = $"ERROR: Destination floor cannot be greater than the top floor {numberOfFloors}.";
            successfull = false;
        }

        if (destinationFloor < 1)
        {
            message = $"ERROR: Destination floor cannot be smaller than the ground floor {1}.";
            successfull = false;
        }

        if (destinationFloor == currentFloor)
        {
            message = $"ERROR: Destination floor cannot be the same as the current floor.";
            successfull = false;
        }
    }
    while (!successfull);

    do
    {
        Console.Clear();
        DisplayElevatorStatus();
        DisplayRequestInformation();
        Console.WriteLine();
        Console.Write($"Createing a elevator call request.\n{message}\nPlease enter number of passengers:\nCurrent Floor: {currentFloor}.\nDestination Floor: {destinationFloor}\n-->");
        successfull = int.TryParse(Console.ReadLine(), out totalPassengers);

        if (!successfull)
            message = "ERROR: Invalid imput. Please enter a valid input.";


        if (totalPassengers < 1)
        {
            message = $"ERROR: Total Passangers cannot be smaller than 1";
            successfull = false;
        }

        if (totalPassengers > maxumumPassengers)
        {
            message = $"ERROR: Total passangers cannot be greater than the maximum allowed passengers: {maxumumPassengers}";
            successfull = false;
        }
    }
    while (!successfull);

    system.Pickup(currentFloor, destinationFloor, totalPassengers);
}
