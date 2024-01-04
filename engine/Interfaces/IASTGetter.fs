namespace Engine
    open Types
    type IASTGetter = 
        abstract member GetAST: string -> Result<Node, string>

