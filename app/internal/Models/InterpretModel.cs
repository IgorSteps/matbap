using System.Xml.Linq;

namespace app
{
    public class InterpretationModel: IInterpretModel
    {
        /// <summary>
        /// Passes expression into maths engine to interpret.
        /// </summary>
        public string Interpret(string expression)
        {
            return $"I am not implemented yet, but here is your expression {expression}";
        }
    }
}
