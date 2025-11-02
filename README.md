# MapleElectionSim

MapleElectionSim is a WPF desktop simulation inspired by Canadian federal election campaigns. The solution is split into projects for the core simulation logic, the presentation layer, and unit tests.

## Projects

- **MapleElectionSim.Core** – Contains the domain models, services, sample data providers, and the `CampaignSimulation` engine that advances campaign state and broadcasts updates via an event bus.
- **MapleElectionSim.App** – WPF MVVM front-end that renders the interactive map, dashboards, riding drill-down panels, and provides timeline controls for driving the simulation.
- **MapleElectionSim.Tests** – xUnit tests that cover the core simulation calculations.

## Features

- Hierarchical geography covering nation, provinces, ridings, and polling divisions.
- Configurable simulation parameters including economic indicators and campaign actions.
- Synthetic loaders for map shapes, demographics, polling, and historical results to keep the app functional without external data files.
- Event-driven updates from the simulation engine to the UI, with start/pause/step controls and JSON save/load support.
- Drill-down dashboards for national metrics, provincial seat outlooks, and riding level demographics and polling trends.

## Getting started

1. Open the `MapleElectionSim.sln` solution in Visual Studio 2022 (17.8 or later) on Windows with the .NET 8.0 SDK installed.
2. Restore NuGet packages and build the solution.
3. Set `MapleElectionSim.App` as the startup project and run. Use the toolbar to start, pause, or step through the simulation and explore the map and detail panels.

## Tests

Run unit tests from Visual Studio Test Explorer or via the command line:

```bash
dotnet test MapleElectionSim.sln
```

## Assets

The `Assets/MapData` folder is a placeholder for shapefiles or GeoJSON that can replace the synthetic geometry provided by `StubMapDataService`.

## License

This repository is provided for demonstration purposes. Update licensing information as appropriate for your project.
