namespace app
{
    public class Expression
    {
        public string Value { get; private set; }
        public Expression(string expression)
        {
            Value = expression;
        }
    }
}
