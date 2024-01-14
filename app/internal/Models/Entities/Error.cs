namespace app
{
    public class Error
    {
        public string Message { get; private set; }

        public Error(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return $"Error: {Message}";
        }
    }
}
