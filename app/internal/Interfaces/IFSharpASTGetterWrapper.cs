namespace app
{
    public interface IFSharpASTGetterWrapper
    {
        public FSharpASTGetterResult GetAST(string expression);
    }
}
