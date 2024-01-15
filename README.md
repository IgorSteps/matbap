# matbap

Maths interpreter software with extensions aiming towards a Turing complete language.

[Jira board](https://liamfarese.atlassian.net/jira/software/projects/AP/boards/2)


[Discovery Board](https://ueanorwich-my.sharepoint.com/:wb:/g/personal/mkq20jzu_uea_ac_uk/Efhj28AX26RPhuUebxabd_gBn3a929Ur_9FcngwqGEKR4w?e=GFHg0L)

## App Spec
UML Diagrams and Report can be found under the [docs](https://github.com/IgorSteps/matbap/tree/main/docs/UML%20Diagrams) folder.

<table>
<tr>
<td valign="top"> <!-- First Column -->
  
### F# Engine
- [ ] Expressions
  - [x] Interger
  - [x] Float
  - [x] Functions
  - [x] Parse Tree
  - [ ] Rational
  - [ ] Complex
- [ ] Assignment
  - [x] Dynamic typing
  - [ ] Static typing
- [ ] Statement for control flow
  - [x] Iteration
  - [ ] Condition
- [ ] Maths toolbox
   - [x] Derivatives
   - [x] Root finding
   - [x] Integration
   - [ ] Matrix and Vector library
- [ ] Turn into a Compiler

</td>
<td valign="top"> <!-- Second Column -->
  
### C# GUI
- [x] Basic GUI
- [x] Basic Plotting
  - [x] Lines
  - [x] Polynomials
- [ ] Advanced Plotting
  - [x] Trig and logarithmic functions
  - [x] Plotting multiple plots
  - [x] Tangents
  - [x] Show areas under the curve
  - [ ] Rational equations
- [ ] Advanced GUI
  - [x] Display Plotted Equations
  - [x] Visualise Parse Tree
  - [x] Visualise Symbol Table

</td>
</tr>
</table>

## Tests
To run all tests, from project root:
```
dotnet test
```

To run just functional tests:
```
 dotnet test --filter FullyQualifiedName~app.Test.Functional
```

To run units tests to the F# engine:
```
 dotnet test --filter FullyQualifiedName~engine.Test
```

To run unit tests for the C# app:
```
 dotnet test --filter FullyQualifiedName~app.Test.Unit
```
