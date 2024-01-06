namespace Engine
    open Types
    /// Interface for evaluating mathematical expressions and plot functions.
    type IEvaluator =
        abstract member Eval: string * SymbolTable -> Result<(string * Points * SymbolTable * Node), string>
        abstract member PlotPoints: float * float * float * string -> Result<float array array, string>


