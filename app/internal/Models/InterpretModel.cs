using Microsoft.FSharp.Core;
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
            var result = Engine.Evaluator.eval(expression);

            return result; 
        }

    }
}
