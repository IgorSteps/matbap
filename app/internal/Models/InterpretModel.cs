using System.Xml.Linq;

namespace app
{ 
    public class InterpretationModel
    {
        /// <summary>
        /// Passes expression into maths engine to interpret.
        /// </summary>
        /// <param name="expression">Arithmetic expression passed into GUI</param>
        /// <returns></returns>
        public string Interpret(string expression)
        {
            return $"I am not implemented yet, but here is your expression {expression}";
        }
    }
}
