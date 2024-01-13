namespace Engine

type IntegratorWrapper() =
    interface IIntegrator with
        member this.Integrate(exp, min, max, step) =
            Integration.integrate exp min max step


