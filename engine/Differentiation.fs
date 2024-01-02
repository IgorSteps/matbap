namespace Engine
    module Differentiation =
        open Types
        
        /// Look Up Table for trigonometric or logarithmic functions.
        let derivativeLUT = dict [
            "sin", (fun x -> Function("cos", x))
            "cos", (fun x -> UnaryMinusOperation("-", Function("sin", x)))
            "tan", (fun x -> BinaryOperation("^", Function("sec", x), Number(Int 2)))
            "log", (fun x -> BinaryOperation("/", Number(Int 1), x))
        ]
        
        /// Differentiate with respect to a variable.
        let rec differentiate (node: Node) (var: string) : Result<Node, string> =
            match node with
            | Number _ -> Ok(Number(Int 0))
            | Variable (varName) -> 
                if varName = var then 
                    Ok(Number(Int 1))    
                else 
                    Ok(Number(Int 0))    
            | BinaryOperation(operation, left, right) ->
                match operation, differentiate left var, differentiate right var with
                // d(u+v)/dx = du/dx + dv/dx
                | "+", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation("+", dLeft, dRight))
                // d(u-v)/dx = du/dx - dv/dx
                | "-", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation("-", dLeft, dRight))
                // d(u*v)/dx = u*dv/dx + v*du/dx
                | "*", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation(
                                                        "+", 
                                                        BinaryOperation("*", dLeft, right),
                                                        BinaryOperation("*", left, dRight)
                                                        )
                                                        )
                // d(u/v)/dx = (v*du/dx - u*dv/dx) / v^2
                | "/", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation(
                                                        "/",
                                                        BinaryOperation(
                                                            "-",
                                                            BinaryOperation("*", dLeft, right),
                                                            BinaryOperation("*", left, dRight)
                                                        ), 
                                                        BinaryOperation("^", right, Number(Int 2))))
                         
                | "^", _, _ ->
                    match left, right with
                        | baseDiff, Number(Int power) ->
                            let newPower = power - 1
                            let newPowerExpr = BinaryOperation("^", baseDiff,  Number (Int newPower))
                            let derivative = BinaryOperation("*", Number (Int power), newPowerExpr)
                            Ok(derivative)
                        | baseDiff, Number(Float power) ->
                            let newPower = power - 1.0
                            let newPowerExpr = BinaryOperation("^", baseDiff,  Number (Float newPower))
                            let derivative = BinaryOperation("*", Number (Float power), newPowerExpr)
                            Ok(derivative)
                        | _, _ -> Error("Differentiation with non-constant power is not supported")
                | _ -> Error(sprintf "Operation '%s' is not supported for differentiation" operation)
            | ParenthesisExpression expression ->
                match differentiate expression var with
                | Ok(dExp)      -> Ok(ParenthesisExpression dExp)
                | Error(msg)    -> Error(msg)
            | UnaryMinusOperation (_, expression) ->
                match differentiate expression var with
                | Ok(dExp)      -> Ok(UnaryMinusOperation("-", dExp))
                | Error(msg)    -> Error(msg)
            | _ -> Error ("Unsupported node type for differentiation")