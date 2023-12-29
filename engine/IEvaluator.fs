namespace Engine
    open System.Collections.Generic
    type SymbolTable = Dictionary<string, Parser.NumType>
        
    /// Interface for evaluating mathematical expressions and plot functions.
    type IEvaluator =
        abstract member Eval: string * SymbolTable -> Result<(string * SymbolTable), string>


