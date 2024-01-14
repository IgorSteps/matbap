namespace Engine
    module Types =
        open System.Collections.Generic

        type NumType =
            | Int of int
            | Float of float

        /// Base type for expression nodes in the AST.
        type Node =
            | Number of NumType
            | BinaryOperation of string * Node * Node
            | ParenthesisExpression of Node
            | UnaryMinusOperation of string * Node
            | VariableAssignment of string * Node
            | Variable of string
            | ForLoop of Node * Node * Node * Node
            | Function of string * Node

        type Points = (float * float) list

        type SymbolTable = Dictionary<string, NumType>

        /// Trapezium vertex with x,y coordinates.
        type Vertex = double * double
        type Vertices = Vertex list
