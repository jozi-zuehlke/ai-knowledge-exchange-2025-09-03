# [Name] Implementation Plan

## Overview

This plan details the implementation of ... following Contract-First Development (CFD) principles.

## Analysis of Existing Code

### Key Components to Reuse

... (if any)

### Technical Debt or Known Issues

- Note any relevant bugs, limitations, or design shortcomings in the current codebase

## Contract-First Development Plan

### 1. Determine The Target Vertical Slice (new or existing)

...

### 2. Define Contracts (CFD)

Add the following contracts to `Contracts.cs`:

- ...

> ⚠️ Consider versioning of contracts if changes may affect existing consumers.

### 3. Write Tests Against Contracts (TDD)

- Define expected success and failure behaviors
- Include edge cases and contract boundary conditions
...

#### Test Cases

- ...

### 4. Run All Tests And Assert That All New Test Cases Are Failing

- Run `just test` to verify all tests fail (red phase)

### 5. Implement Business Logic

...

#### Optional: Design Sketch

- (If complex) Add a brief diagram or sequence sketch to clarify the implementation structure.

#### Optional: Configure Middleware / Pipeline

- If new behaviors require it, update request pipelines, middleware, or handlers accordingly.

### 6. Add Services To DI

...

### 7. Run All Tests And Fix Code Until All Tests Succeed

- Run `just test` to verify implementation (green phase)
- Fix any failing tests
- You MUST ensure that all tests succeed, the functionality is not considered to be finished otherwise

### 8. Refactor And Polish

- Run `just format` and `just fix` for code quality
- Optimize quality attributes like security, performance, reliability, observability, etc. if needed

### 9. Update Documentation

- Create or update `ARCHITECTURE.md` for the vertical slice
- Update system-level documentation (e.g. top-level `SYSTEM_ARCHITECTURE.md`, sequence diagrams if communication patterns changed, etc.)

## Implementation Checklist

1. [ ] ...
2. [ ] ...

> Note that this list may be extendend during development

## Dependencies

- ...

## Constraints

- Must follow CFD/TDD development style
- Must use Conqueror message pattern instead of direct service calls
- Must follow existing code style and patterns
- Must use end-to-end testing approach
- ...
