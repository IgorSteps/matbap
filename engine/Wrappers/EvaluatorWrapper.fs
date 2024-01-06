namespace Engine
    
    /// Implements IEvaluator Interface.
    type EvaluatorWrapper() =
        interface IEvaluator with
            
            /// Evaluate mathematical expression.
            member this.Eval(exp, symTable) = 
                ASTEvaluator.evalToString exp symTable

            /// Evaluate plot function.
            member this.PlotPoints(min, max, step, exp) = 
                ASTEvaluator.plotPoints min max step exp