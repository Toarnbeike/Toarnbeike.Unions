using Toarnbeike.Unions.SourceGenerator;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

[UnionCase(typeof(Active))]
[UnionCase(typeof(Retry))]
[UnionCase(typeof(Aborted))]
public partial class Status;