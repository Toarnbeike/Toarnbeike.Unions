IsExternalInit is geen runtime dependency maar een compile-time type dat de compiler nodig heeft om record en init te begrijpen.

3. Intern NuGet package (geavanceerd)

Je kunt een package maken dat via buildTransitive automatisch dit bestand injecteert:

buildTransitive/
  IsExternalInit.props
  IsExternalInit.cs

✔ Wordt automatisch toegevoegd aan consuming projects
✔ Geen handmatige includes nodig

Dit is hoe veel libraries het oplossen.

Als je straks toch een NuGet package maakt voor je generator tooling, is optie 3 de moeite waard — dat maakt je developer experience aanzienlijk beter.