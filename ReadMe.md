
# Elevator Simulator

The Elevator Simulator is an application built to simulate scheduling and dispatch of elevators in a building. With this application, your can can explore the intricacies of elevator management, and view how elevators efficiently respond to calls, optimize routes, and handle varying traffic demands. In order to efficiently select the most suitable elevator, the simulator utilizes a custom algorithm that takes three main factors into consideration namely;

* Distance from the elevator requester.
* Current direction the elevator is taking.
* Remaining load capacity.

The reason for using these considerations as part of the scoring algorithm was to ensure that there was room to improve both on wait times as well as transport as much load (passengers or freight) as possible in a single trip. The algorithm utilizes the total number of floors as a standardizing mechanism to ensure all scoring factors have a common base.






## Run Locally

Clone the project

```bash
  git clone https://github.com/CodeLover254/ElevatorSimulator.git
```

Go to the project directory

```bash
  cd ElevatorSimulator/src
```

Restore dependencies

```bash
  dotnet restore
```

Build the application

```bash
  dotnet build --configuration Release
```

Run application

```bash
  cd ElevatorSimulator.Console/bin/Release/net6.0
  dotnet run ElevatorSimulator.Console.dll
```


## Running Tests

To run tests, run the following command

```bash
  dotnet tests
```

