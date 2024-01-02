﻿namespace Engine
    module Differentiation =
        open Types
        
        /// Look Up Table for trigonometric or logarithmic functions.
        let derivativeLUT = dict [
            "sin", (fun x -> Function("cos", x))
            "cos", (fun x -> UnaryMinusOperation("-", Function("sin", x)))
            "tan", (fun x -> BinaryOperation("^", Function("sec", x), Number(Int 2)))
            "log", (fun x -> BinaryOperation("/", Number(Int 1), x))
            "ln", (fun x -> BinaryOperation("/", Number(Int 1), x))
            "exp", (fun x -> Function("exp", x))
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
                // Sum rule.
                | "+", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation("+", dLeft, dRight))
                // Difference rule.
                | "-", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation("-", dLeft, dRight))
                // Product rule.
                | "*", Ok(dLeft), Ok(dRight) -> Ok(BinaryOperation(
                                                        "+", 
                                                        BinaryOperation("*", dLeft, right),
                                                        BinaryOperation("*", left, dRight)
                                                        )
                                                        )
                // Quotent rule.
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
            | Function(funcName, innerFunc) ->
                if derivativeLUT.ContainsKey(funcName)
                    then
                        let outerDerivative = derivativeLUT.[funcName](innerFunc)
                        chainRule outerDerivative innerFunc var
                    else
                        Error(sprintf "Function '%s' is not supported for differentiation" funcName)
            | _ -> Error ("Unsupported node type for differentiation")

        and chainRule outerDerivativeFunc innerFunc var =
            match differentiate innerFunc var with
            | Ok(innerDerivative) -> Ok(BinaryOperation("*", outerDerivativeFunc, innerDerivative))
            | Error(msg) -> Error(msg)
