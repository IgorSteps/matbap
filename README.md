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
  - [ ] GPU speed up
  - [x] Zoom in/out
  - [x] Translate the axis
  - [ ] Lines/polynomials extend indefinitely
  - [ ] Plotting multiple plots
  - [ ] Lines get translated to whatever scale the axis are


## Tests
To run unit and funtional tests, from project root:
```
dotnet test
```
