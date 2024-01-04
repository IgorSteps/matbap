using Microsoft.Msagl.Drawing;
using static app.ASTManager;
using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public interface IASTConverter
    {
        public ConvertionResult Convert(FSharpASTNode fSharpNode);
        public string ConvertToString(ASTNode root);
        public Graph ConvertAstToGraph(ASTNode root);
    }
}
