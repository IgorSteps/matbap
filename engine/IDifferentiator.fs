namespace Engine
    open Types

    type IDifferentiator = 
        abstract member Differentiate: Node * string -> Result<Node, string>
