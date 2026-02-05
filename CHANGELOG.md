# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](https://semver.org/).

---

<a id="v2-0-0"></a>
## [2.0.0] - 2026-02-##

### Added
- Support for source generated nominal unions.
  - [UnionCase(...)] with record support;
  - Extensions: Match, Switch, Map, Bind, Tap
  - TestExtensions: ShouldBe
  - CollectionExtensions: Select and Partition

- Included analyzers
  - A union must declare at least two cases
  - Union case must be unique
  - Union case must be a valid type
  - Union declaration must be partial
  - Union case must be concrete
  - Union case must be non generic
  - Union case must be unnested

- Tests on nominal source generated union types

<a id="v1-3-0"></a>
## [1.3.0] - 2026-02-01

### Changed
- All `Union<T1 ... Tn>` methods and extensions are moved to the `Toarnbeike.Unions.Generic` namespace.
  This gives space for the source generated Unions to go to the `Toarnbeike.Unions.Nominal` namespace.
  By applying this change now before the Nominal extensions are out, the migration is much easier.

#### Migration
- Replace all `using Toarnbeike.Unions.*` with `using Toarnbeike.Unions.Generic.*`.

### Fixed
- Missing closings of Xml comments.
- Mapping parameter names
- Convert into method group where possible.

<a id="v1-2-0"></a>
## [1.2.0] - 2025-12-30

### Changed
- The Map(...) extension once again supports transformations that change the resulting Union type.

  This reverts the behavioral restriction introduced in [`v1.1.0`](#v1-1-0). The decision was made to preserve conceptual consistency with `Map` semantics across the other Toarnbeike functional packages, where `Map` is allowed to project values to new types.

  In the upcoming source generator release, two `Map` overloads will be available:

  - A `Map` overload that preserves the shape of the generated union and returns the original generated union type.
  - A `Map` overload that allows projecting to new types and returns a `Union<...>` with the resulting type arguments.

This approach was chosen over maintaining a separate `Project(...)` method, in order to keep naming consistent and intuitive across the functional API surface.

### Removed
- Removed the `Project(...)` extension method introduced in `v1.1.0`

#### Migration
Replace all usages of `Project(...)` with `Map(...)` functions. The `Map(...)` overloads accept the same parameters and provide equivalent behavior.

<a id="v1-1-1"></a>
## [1.1.1] - 2025-12-19

### Tooling
- CI/CD improvements
- Version checks

<a id="v1-1-0"></a>
## [1.1.0] - 2025-12-19

### Added
- New `Project(...)` extension method for projecting a Union to a new shape

### Changed
- `Map(...)` is now strictly shape-preserving and no longer allows changing the Union shape

#### Migration
Use `Project(...)` instead of `Map(...)` when transforming to a different Union shape:

```csharp
// Before
union.Map(
    t1 => Union<A,B>.FromT1(...),
    t2 => Union<A,B>.FromT2(...)
);

// After
union.Project(
    t1 => Union<A,B>.FromT1(...),
    t2 => Union<A,B>.FromT2(...)
);
```

<a id="v1-0-0"></a>
## [1.0.0] - 2025-12-18

### Added
- Initial release of `Toarnbeike.Unions`
- Immutable, allocation-free `Union<T1..Tn>` types
- Functional extension methods: Match, Switch, Map, Bind, Tap
- Async and `Task<Union<...>>` overloads
- Collection extensions and test extensions