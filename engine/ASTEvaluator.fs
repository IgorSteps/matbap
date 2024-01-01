namespace Engine
    module ASTEvaluator =
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
            // Derivative of a number is 0.
            | Number _ -> Ok(Number(Int 0))
            | VariableAssignment(name, expression) -> 
                if name = var then 
                    // Differentiate the expression with respect to 'var'.
                    differentiate expression var  
                else 
                    // Treat it as a number, derivative is 0.
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
                                                        BinaryOperation("*", right, right)))
                 
                | "^", Ok(diffBase), Ok(Number (Int power)) ->
                    let newPower = power - 1
                    let newPowerExpr = BinaryOperation("^", left,  Number (Int power))
                    let derivative = BinaryOperation("*", newPowerExpr, BinaryOperation("*", Number (Int power), diffBase))
                    Ok(derivative)
                | _ -> Error("Unsupported operation for differentiation")
            | ParenthesisExpression expression ->
                match differentiate expression var with
                | Ok(dExp)      -> Ok(ParenthesisExpression dExp)
                | Error(msg)    -> Error(msg)
            | UnaryMinusOperation (_, expression) ->
                match differentiate expression var with
                | Ok(dExp)      -> Ok(UnaryMinusOperation("-", dExp))
                | Error(msg)    -> Error(msg)
            | Derivative (expression, diffVar) -> differentiate expression diffVar
            | _ -> Error ("Unsupported node type for differentiation")


