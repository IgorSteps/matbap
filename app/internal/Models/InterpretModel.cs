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

            if (!result.IsOk) {
                return result.ErrorValue as string;
            }
            // Item1 is result of expression, Item2 is symTable
            return result.ResultValue.Item1 as string;

        }

    }
}
