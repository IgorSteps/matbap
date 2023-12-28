# matbap

Maths interpreter software with extensions aiming towards a Turing complete language.

[Jira board](https://liamfarese.atlassian.net/jira/software/projects/AP/boards/2)


[Discovery Board](https://ueanorwich-my.sharepoint.com/:wb:/g/personal/mkq20jzu_uea_ac_uk/Efhj28AX26RPhuUebxabd_gBn3a929Ur_9FcngwqGEKR4w?e=GFHg0L)

## App Spec
### F# Engine
- [ ] Expressions
  - [x] Interger
  - [x] Float
  - [x] Functions
  - [ ] Rational
  - [ ] Complex
  - [ ] Parse Tree
- [ ] Assignment
  - [x] Dynamic typing
  - [ ] Static typing
- [ ] Statement for control flow
  - [ ] Iteration
  - [ ] Condition
- [ ] Maths toolbox
   - [ ] Derivatives
   - [ ] Root finding
   - [ ] Integration
   - [ ] Matrix and Vector library
- [ ] Turn into a Compiler

### C# GUI
- [x] Basic GUI
- [x] Basic Plotting
  - [x] Lines
  - [x] Polynomials
- [ ] Advanced Plotting
  - [x] Trig and logarithmic functions
  - [x] Plotting multiple plots
  - [ ] Rational equations
  - [x] Tangents
  - [ ] Ereas under the curve for numerical integration.
- [ ] Advanced GUI
  - [x] Visualise Parse Tree
  - [ ] Visualise Symbol Table
  - [x] Display Plotted Equations



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