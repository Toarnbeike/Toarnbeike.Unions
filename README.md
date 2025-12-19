![CI](https://github.com/Toarnbeike/Toarnbeike.Functional/actions/workflows/build.yaml/badge.svg)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# Toarnbeike.Unions

This package provides **immutable, allocation-free union types** for C#, inspired by functional programming and discriminated unions, while remaining idiomatic to the .NET ecosystem.

A `Union<T1, …, Tn>` represents a value that can be exactly one of several possible types at runtime, without relying on inheritance, nulls, or exceptions for control flow.
This is particularly useful for modeling domain states and workflows where a value can be in one of a fixed set of alternatives.

## Features
- Generated Union types with 2 to 8 type parameters (expandable via included generators)
- Explicit construction and state inspection
- Rich functional extensions (Map, Bind, Match, Switch, Tap)
- Async support for all extensions
- Collection extensions for partitioning
- Test extensions for fluent assertions
- Comprehensive XML documentation and usage examples
- Unit tested with high code coverage

---

## Contents
1. [Quick start](#quick-start)
1. [Core concepts](#core-concepts)
1. [Extensions](#extensions)
1. [Collections](#collections)
1. [Test extensions](#test-extensions)
1. [Design principles](#design-principles-and-best-practices)
1. [Comparison: Why Unions?](#comparison-why-unions)
1. [When not to use Unions?](#when-not-to-use-unions)
1. [Conclusion](#conclusion)

---

## Quick start

This example demonstrates the most common workflow when using unions: construction, transformation, and consumption.

```csharp
using Toarnbeike.Unions;
using Toarnbeike.Unions.Extensions;

Union<int, string> value = Union<int, string>.FromT1(10);

// Transform the value
var transformed = value
    .Map(
        t1 => t1 + 1,
        t2 => t2.Length
    );

// Consume the union
var result = transformed.Match(
    t1 => $"Int: {t1}",
    t2 => $"String length: {t2}"
);

Console.WriteLine(result); // Output: Int: 11
```

Key properties of this workflow:

- The union is always in exactly one state
- All transformations are explicit
- All states must be handled exhaustively
- No nulls, casts, or exceptions are required

---

## Core concepts

### What is a Union?

A union represents **one of several possible states**, each carrying a value of a different type.

```csharp
Union<int, string> value = Union<int, string>.FromT1(42);
// or
Union<int, string>.FromT2("hello");
```

At any point in time, a union is in **exactly one state.**

### Construction

Unions are constructed via explicit static factory methods:

```csharp
var u1 = Union<int, string>.FromT1(10);
var u2 = Union<int, string>.FromT2("text");
```

This ensures:
- No ambiguous states
- No implicit conversions
- No null-based discrimination

### State Inspection

Each union exposes state checks:
```csharp
if (union.IsT1) { ... }
if (union.IsT2) { ... }
```

Safe extraction using TryGet is available; TryGet never throws and does not allocate.
```csharp
if (union.TryGetT1(out T1 value)) { ... }
```

---

## Extensions

The `Toarnbeike.Unions.Extensions` namespace contains rich extension methods for all `Union<T1,...,Tn>`.

| Method         | Returns       | Description                                          | 
|----------------|---------------|------------------------------------------------------| 
| `Match(...)`   | `TResult`     | Consume the union by handling all possible states.   | 
| `Switch(...)`  | `void`        | Handle a side effect for each possible state.        | 
| `Map(...)`     | `Union<...>`  | Transform one or all values within the same Union.   | 
| `Project(...)` | `Union<...>`  | Projects one or all values to a new Union.           |
| `Bind(...)`    | `Union<...>`  | State-dependent transitions to another `Union<>`.    | 
| `Tap(...)`     | `Union<...>`  | Side-effects for a specific state (fluent).          | 

All extensions include async overloads and `Task<Union<...>>` variants.

For information per method, see the [Extensions README](docs/Extensions.md).

---

## Collections

The `Toarnbeike.Unions.Collections` namespace contains extension methods to work with `IEnumerable<Union<...>>`.

This adds the possibility to split a collection of unions in separate collections of the different types present:

| Method            | Returns                        | Description                                               |
|-------------------|--------------------------------|-----------------------------------------------------------|
| `SelectTi(...)`   | `IEnumerable<Ti>`              | Select all instances of `Ti` in the `IEnumerable<Union>`. |
| `Partition(...)`  | `Tuple<IReadOnlyList<Ti..Tn>>` | Tuple with an `IReadOnlyList` of instances of each type.  |

### SelectTi
Select Ti extracts all values of type Ti from a collection of unions.

```csharp
private IEnumerable<Union<U1,U2>> CreateMixedCollection()
{
    yield return Union<U1,U2>.FromT1(new U1(1));
    yield return Union<U1,U2>.FromT1(new U1(2));
    yield return Union<U1,U2>.FromT2(new U2(2));
    yield return Union<U1,U2>.FromT2(new U2(3));
}

var t1Values = CreateMixedCollection().SelectT1().ToList();

t1Values.Count.ShouldBe(2);
t1Values[0].Value.ShouldBe(1);
t1Values[1].Value.ShouldBe(2);
```

### Partition
Partition splits a collection of unions into separate collections for each type.
This is useful when you need to process each union case in bulk.

```csharp
var source = CreateMixedCollection();

var (t1Values, t2Values) = source.Partition();

t1Values.Count.ShouldBe(2);
t2Values.Count.ShouldBe(2);
```

---

## Test extensions

The `Toarnbeike.Unions.TestExtensions` namespace contains extensions for asserting on Unions in tests (framework-agnostic).

Per arity the following methods are available:

| Method            | Returns       | Description                                          |
|-------------------|---------------|------------------------------------------------------|
| `ShouldBeTi()`    | `Ti`          | Asserts the union is in state Ti and returns value.  |
| `ShouldBeTi(Ti)`  | `Ti`          | Asserts the union is in state Ti with expected value.|

### Usage
```csharp
var t1 = union.ShouldBeT1();
var actual = union.ShouldBeT1(expectedValue);
```

---

## Design principles and best practices

This library intentionally favors:
- Explicit over implicit
- Composition over convenience
- Minimal primitives over combinator explosion
- Functional correctness over syntactic shortcuts

Notably **out of scope**:
- Implicit conversions (types might not be unique within a union)
- Null-based semantics (use [Options](https://github.com/Toarnbeike/Toarnbeike.Optional))

---

## Comparison: Why Unions?

| Alternative        | Typical use case               | How unions differ                                     |
|--------------------|--------------------------------|-------------------------------------------------------|
| Inheritance        | Behavioral polymorphism        | Unions model closed, explicit state alternatives.     |
| Enums              | State without data             | Unions associate each state with its own value.       |
| Nulls              | Optional references            | Unions encode alternatives explicitly.                |
| Exceptions         | Unexpected failures            | Unions model expected outcomes.                       |
| Option / Result    | Presence / success vs failure  | Unions generalize this to multiple meaningful states. |

For a more detailed comparison, see the [Comparison document](docs/Comparison.md).

---

## When not to use Unions?

| Scenario                               | Prefer instead           | Why                                                       |
|----------------------------------------|--------------------------|-----------------------------------------------------------|
| Simple success / failure               | `Result<T>`              | Binary outcomes are clearer with a dedicated abstraction. |
| Optional presence only                 | `Option<T>`              | Avoid encoding absence as an extra union case.            |
| Open-ended or extensible hierarchies   | Inheritance / interfaces | Unions are intentionally closed.                          |
| Cross-cutting behavioral polymorphism  | Interfaces               | Unions model data states, not shared behavior.            |
| Performance-critical hot paths (micro) | Specialized structs      | Pattern matching may be unnecessary overhead.             |

Unions are a powerful modeling tool, but they are intentionally not universal.
 
---

## Conclusion

`Union<T1, …, Tn>` enables:
- Strongly typed alternatives
- Exhaustive handling
- Functional transformations
- Explicit side effects
- Predictable async composition

All while remaining:
- Immutable
- Allocation-efficient
- Generator-friendly
- Testable

This makes Unions suitable for **domain modeling, state machines, and functional workflows** in modern C#.