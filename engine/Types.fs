namespace Engine
    module Types =
        type NumType =
            | Int of int
            | Float of float

        // Base type for expression nodes in the AST.
        type Node =
            | Number of NumType
            | BinaryOperation of string * Node * Node
            | ParenthesisExpression of Node
            | UnaryMinusOperation of string * Node
            | VariableAssignment of string * Node
