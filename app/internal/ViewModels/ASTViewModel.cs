using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public class ASTViewModel : ObservableObject
    {
        private ASTNode _rootNode;
        public ObservableCollection<ASTNode> ASTNodes { get; } = new ObservableCollection<ASTNode>();

        public ASTViewModel()
        {
            // Manual creation.
            var input = "1 + 2 * 3 / (4 - 5) ^ 6 % 7";
            var tokens = Engine.Tokeniser.tokenise(input);
            var ast = Engine.ASTParser.parse(tokens.ResultValue);
            FSharpASTNode fSharpAST = ast.ResultValue;

            // Convert F# AST to C# AST
            var converter = new ASTConversionService();
            RootNode = converter.Convert(fSharpAST);
            ASTNodes.Clear();
            ASTNodes.Add(RootNode);
        }

        public ASTNode RootNode
        {
            get => _rootNode;
            set => SetProperty(ref _rootNode, value);
        }
    }
}
