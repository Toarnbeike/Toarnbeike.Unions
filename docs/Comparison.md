Union types solve a different problem than inheritance, enums, or exceptions.
Below is a brief comparison of common alternatives.

### Inheritance / Polymorphism

Inheritance models *behavioral hierarchies*.
Unions model *explicit state alternatives*.

Inheritance:
- Allows implicit state extension
- Often requires downcasting or virtual dispatch
- Makes exhaustive handling non-trivial

Unions:
- Represent a closed set of states
- Enforce exhaustive handling via `Match`
- Avoid base-class coupling

Use inheritance when:
- You model shared behavior
- You expect open-ended extension

Use unions when:
- States are known and finite
- Each state represents a distinct meaning

### Enums + Switch

Enums represent state, but not state data.

Enums:
- Separate state from data
- Require additional storage for associated values
- Rely on convention for correctness

Unions:
- Combine state and value
- Prevent invalid combinations
- Make illegal states unrepresentable

Use enums when:
- State has no associated data

Use unions when:
- Each state carries its own value

### Nulls

Nulls represent *absence*, not *alternatives*.

Null-based code:
- Is not self-documenting
- Shifts responsibility to the caller
- Fails at runtime instead of compile time

Unions:
- Make alternatives explicit
- Encode intent in the type system
- Fail fast at compile time

Use nulls for:
- Truly optional references (or, even better, use my [Options package](https://github.com/Toarnbeike/Toarnbeike.Optional))

Use unions for:
- Meaningful alternative states

### Exceptions

Exceptions represent *unexpected failure*, not control flow.

Exception-based flow:
- Is non-local
- Obscures control paths
- Complicates async reasoning

Unions:
- Model success and failure explicitly
- Compose naturally
- Work predictably with async code

Use exceptions for:
- Programmer errors
- Infrastructure failures

Use unions for:
- Domain-level outcomes
- Expected alternative results