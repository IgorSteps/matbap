namespace Engine

    module ASTGetter =
        open Types
        let private getStrFromLexerError (err : Tokeniser.LexicalError) : string =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse (tokens: Tokeniser.Token list) : Result<Node,string> =
            match ASTParser.parse tokens with
            | Error parseError  -> Error parseError
            | Ok tokens         -> Ok tokens

        let getAST (expression : string) : Result<Node, string> =
            match Tokeniser.tokenise expression with
            | Ok tokens -> match parse tokens with  
                           | Ok tree -> Ok tree
                           | Error e -> Error e
            | Error e -> Error (getStrFromLexerError e)