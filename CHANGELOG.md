# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added
### Changed
### Deprecated
### Removed
### Fixed

---

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

## [1.0.0] - 2025-12-18

### Added
- Initial release of `Toarnbeike.Unions`
- Immutable, allocation-free `Union<T1..Tn>` types
- Functional extension methods: Match, Switch, Map, Bind, Tap
- Async and `Task<Union<...>>` overloads
- Collection extensions and test extensions