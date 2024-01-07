namespace Engine
    type RootFinderWrapper() =
        interface IRootFinder with
            member this.FindRoots(xmin, xmax, exp) =
                ASTEvaluator.findRoots xmin xmax exp

