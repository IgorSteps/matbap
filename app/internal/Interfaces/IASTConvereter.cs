using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public interface IASTConverter
    {
        public ASTNode Convert(FSharpASTNode fSharpNode);
        public string ConvertToString(ASTNode root);
    }
}
