# Toarnbeike.Unions.Extensions 

This namepace provides extension methods for working with Union types, enabling functional programming patterns such as mapping, binding, matching, and switching on union values. 

These extensions are inspired by functional programming paradigms and are designed to work seamlessly with the immutable, allocation-free Union types provided by the Toarnbeike.Unions library. 

--- 

## Overview 

| Method                   | Returns          | Description                                          | 
|--------------------------|------------------|------------------------------------------------------| 
| [`Match(...)`](#match)   | `TResult`        | Consume the union by handling all possible states.   | 
| [`Switch(...)`](#switch) | `void`           | Handle a side effect for each possible state.        | 
| [`Map(...)`](#map)       | `Union<...>`     | Transform one or all values within the same Union.   | 
| [`Bind(...)`](#bind)     | `Union<...>`     | State-dependent transitions to another `Union<>`.    | 
| [`Tap(...)`](#tap)       | `Union<...>`     | Side-effects for a specific state (fluent).          | 

### Choosing the right method

- Use [`Match`](#match) when you want a final result
- Use [`Switch`](#switch) for terminal side effects
- Use [`Map`](#map) when transforming values
- Use [`Bind`](#bind) when the next state depends on the current value
- Use [`Tap`](#tap) for side effects while keeping the union

--- 

## Match 

The Match extension method allows you to consume a union by providing handlers for each possible state. 
It returns a result based on the state of the union.
`Match` is a terminal operation: it consumes the union and does not allow further chaining.

```csharp
var result = union.Match(
	t1 => $"It's an int: {t1}",
	t2 => $"It's a string: {t2}"
);
```

Overloads are available for all Union types, including async variants and `Task<Union<...>>` versions. 

--- 

## Switch 

The Switch extension method allows you to perform side effects based on the state of the union without returning a value.
`Match` is a terminal operation: it consumes the union and does not allow further chaining.
For chaining side-effects while retaining the union, consider using [`Tap`](#tap).

```csharp
union.Switch(
	t1 => Console.WriteLine($"It's an int: {t1}"),
	t2 => Console.WriteLine($"It's a string: {t2}")
);
```

Overloads are available for all Union types, including async variants and `Task<Union<...>>` versions. 

--- 

## Map 

The `Map` extension method allows you to transform the value contained in a union **while preserving the union context**.

This makes Map a non-consuming, that can be fluently chained and safely used in domain models.

```csharp
Union<double, string> newUnion = union.Map(
	t1 => t1 / 2.0d,           // Transform int, becomes a double
	t2 => t2.ToUpper()         // Transform string, stays a string
);
```

Besides the global mapping of all types, Map also supports mapping individual types while leaving others unchanged:

```csharp
var newUnion = union.MapT1(
	t1 => t1 / 2.0d           // Transform int, leafs T2 (string) unmodified.
);
```

Overloads are available for all Union types, including async variants and `Task<Union<...>>` versions. 

--- 

## Bind 

The Bind extension method allows you to perform case-dependent transitions to other cases within the same union. 
This is useful for e.g. a state machine, where an operation may result in different union cases based on the current state.

```csharp
var newUnion = union.Bind(
	t1 => Union<int, string>.FromT2("Converted from int"),
	t2 => Union<int, string>.FromT1(t2.Length)
);
```

Besides the global binding of all types, Bind also supports binding individual types while leaving others unchanged:

```csharp
var newUnion = union.BindT1(
	t1 => Union<int, string>.FromT2("Converted from int") // Only bind int, leave string unchanged
);
```

Overloads are available for all Union types, including async variants and `Task<Union<...>>` versions. 

--- 

## Tap 

The Tap extension method allows you to perform side effects for specific states of the union while retaining the original union.

```csharp
var result = union
	.TapT1(t1 => Console.WriteLine($"Tapped int: {t1}"))
	.TapT2(t2 => Console.WriteLine($"Tapped string: {t2}"))
);
```

Overloads are available for all Union types, including async variants and `Task<Union<...>>` versions. 

---