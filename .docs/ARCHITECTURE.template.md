# [Name] Vertical Slice Architecture

This slice follows the vertical slice architecture pattern, containing all the code related to the [Name] functionality in a single directory.

## Components

- **`Contracts.cs`**: Defines the Conqueror message / signal contracts for ...

## Contracts

- **`MyExampleMessage`**: this message triggers the [description] functionality and ...
- **`MyExampleSignal`**: this signal is published when ...

## Business Logic

The `MyExampleHandler` contains the core logic for ...

## Cross-Cutting Concerns

- **Logging**: We log only through Conqueror middleware pipelines, and other than that we produce user-friendly output directly via `IConsole`
- **Auth**: ...
- **Retry**: ...

## Flow Diagram (Optional)

> insert Mermaid diagram here

Shows how the message flows through the handler, services, and external dependencies.

## Dependencies

- `IMyDataStoreClient`: Fetches ...
- `ILogger<MyExampleHandler>`: Used for ...

## Design Decisions

...

## Known Limitations

- ...

## Testing

The feature is thoroughly tested with end-to-end tests that cover:

- ...

All tests use the `TestHost` infrastructure to execute the full application as a user would, ensuring realistic behavior validation.

## Related Documents

- [Vertical Slice Requirements](./REQUIREMENTS.md)
- ...
