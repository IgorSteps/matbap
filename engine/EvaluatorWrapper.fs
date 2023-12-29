namespace Engine

    type EvaluatorWrapper() =
        interface IEvaluator with
            member this.Eval(exp, symTable) = 
                Evaluator.eval exp symTable