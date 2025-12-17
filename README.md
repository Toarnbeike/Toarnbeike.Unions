![CI](https://github.com/Toarnbeike/Toarnbeike.Functional/actions/workflows/build.yaml/badge.svg)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# Toarnbeike.Unions

This package provides **immutable, allocation-free union types** for C#, inspired by functional programming and discriminated unions, while remaining idiomatic to the .NET ecosystem.

A `Union<T1, …, Tn>` represents a value that can be exactly one of several possible types at runtime, without relying on inheritance, nulls, or exceptions for control flow.

## Features
- Generated Union types with 2 to 8 type parameters (expandable with provided static generators)
- Explicit construction and state inspection
- Rich functional extensions (Map, Bind, Match, Switch, Tap)
- Async support for all extensions
- Collection extensions for partitioning
- Test extensions for fluent assertions
- Comprehensive XML documentation and usage examples
- Unit tested with high code coverage

---

## Contents
1. [Core concepts](#core-concepts)
1. [Extensions](#extensions)
1. [Collections](#collections)
1. [Test extensions](#test-extensions)
1. [Design principles](#design-principles-and-best-practices)
1. [Why Unions?](#why-unions)

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

Safe extraction using TryGet is available:
```csharp
if (union.TryGetT1(out T1 value)) { ... }
```

---

## Extensions

The `Toarnbeike.Unions.Extensions` namespace contains rich extension methods for all `Union<T1,...,Tn>`.

| Method         | Returns       | Description                                         |
|----------------|---------------|-----------------------------------------------------|
| `Match(...)`   | `TResult`     | Consome the union by handling all possible states.  |
| `Switch(...)`  | `void`        | Handle a side effect for each possible state.       |
| `Map(...)`     | `Union<TNew>` | Transform one or all values while returing a Union. |
| `Bind(...)`    | `Union<>`     | State-dependent transitions to another `Union<>`.   |
| `Tap(...)`     | `Union<>`     | Side-effects for a specific or for each state.      | 

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
- Null-based semantics (use [Options](Option.md))

---

## Why Unions?

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