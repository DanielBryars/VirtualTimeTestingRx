# VirtualTimeTestingRx

Supporting code for a .NET meetup talk demonstrating virtual time testing with Reactive Extensions (Rx.NET).

## Overview

This project demonstrates how to use Reactive Extensions' TestScheduler to perform deterministic time-based testing of reactive streams without waiting for real time to pass. The code showcases testing heartbeat monitoring and timeout detection using virtual time.

## Concept

Traditional time-based testing requires actual waiting, making tests slow and non-deterministic. Rx.NET's `TestScheduler` allows you to:
- Instantly "fast-forward" through time
- Test time-dependent behavior deterministically
- Run long-duration scenarios in milliseconds
- Verify timeout and interval-based logic

## Example Scenario

The code demonstrates a heartbeat monitoring system that:
1. Generates heartbeat events at varying intervals (2 or 5 seconds)
2. Detects lost contact when no heartbeat is received for 3 seconds
3. Uses virtual time to test the entire scenario instantly

## Key Components

### TestScheduler
```csharp
TestScheduler scheduler = new TestScheduler();
scheduler.AdvanceTo(DateTime.Now.Ticks);
scheduler.Start();
```

### Heartbeat Stream
- Generates 15 heartbeats with varying intervals
- Some intervals are 2 seconds, others are 5 seconds
- Simulates realistic server communication patterns

### Lost Contact Detection
```csharp
.Buffer(TimeSpan.FromSeconds(2), scheduler)
.Where(b => b.Count == 0)
```
Monitors for gaps in heartbeats and emits `HeartbeatLost` events when contact is lost.

## Technologies

- **.NET Framework**: C# application
- **Reactive Extensions (Rx.NET)**: Microsoft.Reactive.Testing
- **MSTest**: Assertion framework for validation

## Running the Code

```bash
cd SupportingCodeForTalk
dotnet build
dotnet run
```

## Test Assertions

The code validates that exactly 3 lost contact events occur during the heartbeat sequence:
```csharp
Assert.AreEqual(3, countOfLostcontacts);
```

## Key Rx Operators Used

- **Observable.Generate**: Creates heartbeat sequence with dynamic timing
- **Timestamp**: Records when events occur
- **TimeInterval**: Measures time between events
- **Buffer**: Groups events within time windows
- **Timeout**: Detects lack of activity (alternative approach shown in comments)

## Testing Benefits

Virtual time testing provides:
- **Speed**: Tests complete instantly regardless of time spans involved
- **Determinism**: Same results every time, no race conditions
- **Debuggability**: Step through time-based logic without waiting
- **Scalability**: Test hours/days/weeks of behavior in milliseconds

## Presentation Context

This code was created to support a .NET meetup talk demonstrating reactive programming patterns and testing strategies. It serves as an educational example of how to test time-dependent reactive systems effectively.

## Observable Patterns Demonstrated

- Event generation with dynamic intervals
- Time-based monitoring and detection
- Stream composition and transformation
- Debugging reactive streams with timestamps and intervals
