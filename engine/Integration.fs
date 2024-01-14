namespace Engine
    module Integration =
        open Types

        /// Calculates the area of a single trapezium under the curve of a given expression.
        /// xLeft and xRight are x coordinates of left and right sides of the trapezium.
        /// step is the distance between xLeft and xRight.
        /// Returns a tuple of trapezium area and its vertices or error.
        let private calculateTrapeziumArea (expression : string) (xLeft : double) (xRight : double) (step : double) : Result<double * Vertices, string> =
            match ASTEvaluator.plotPoints xLeft xLeft 1.0 expression with
            | Ok array ->
                   let yLeft = array.[0].[1]
                   match ASTEvaluator.plotPoints xRight xRight 1.0 expression with
                   | Ok array ->
                        let yRight = array.[0].[1]
                        let area = (yLeft + yRight) * step / 2.0
                        let vertices = [
                            (xLeft, 0.0);
                            (xLeft, yLeft);
                            (xRight, yRight);
                            (xRight, 0.0)
                        ]
                        Ok (area, vertices)
                   | Error err -> Error err
            | Error err -> Error err

        /// Recursively sums the areas of trapeziums from the current x to the max x.
        /// totalArea accumulates the total area under the curve.
        let rec private sumTrapeziumAreas (totalArea : double) (xCurrent : double) (xMax : double) (step : double) (expression : string) (totalVerticies : Vertices) : Result<double * Vertices, string> =
            if xCurrent >= xMax then
                Ok (totalArea, totalVerticies)
            else
                match calculateTrapeziumArea expression xCurrent (xCurrent + step) step with
                | Ok (area, verticies) ->
                    // Append the vertices to the totalVerticies
                    let updatedVertices =  totalVerticies @ verticies
                    sumTrapeziumAreas (totalArea + area) (xCurrent + step) xMax step expression updatedVertices
                | Error err -> Error err

        /// Approximate the integral of a function using the trapezium rule.
        /// step is the width of each trapezium.
        /// Returns a tuple of area unde the curve and the list of vertices for every trapezium or error.
        let integrate (expression : string) (min : double) (max : double) (step : double) : Result<double * Vertices, string> =
             match sumTrapeziumAreas 0.0 min max step expression [] with
             | Ok (area, verticies) -> Ok (area, verticies)
             | Error err -> Error err
