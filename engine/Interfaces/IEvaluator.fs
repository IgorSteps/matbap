﻿namespace Engine
    open System.Collections.Generic
    open ASTEvaluator
    open Types
    /// Interface for evaluating mathematical expressions and plot functions.
    type IEvaluator =
        abstract member Eval: string * SymbolTable -> Result<(string * SymbolTable * Node), string>
        abstract member PlotPoints: float * float * float * string -> Result<float array array, string>

