using Microsoft.FSharp.Core;
using System.Collections.Generic;
using System.Xml.Linq;

namespace app
{
    public class InterpretationModel: IInterpretModel
    {
        private Dictionary<string, Engine.Parser.NumType> _symTable;

        public InterpretationModel() {
            _symTable = new Dictionary<string, Engine.Parser.NumType>();
        }
        /// <summary>
        /// Passes expression into maths engine to interpret.
        /// </summary>
        public string Interpret(string expression)
        {
            var result = Engine.Evaluator.eval(expression, _symTable);

            if (!result.IsOk) {
                return result.ErrorValue as string;
            }
            // Item1 is result of expression, Item2 is symTable
            _symTable = result.ResultValue.Item2;
            return result.ResultValue.Item1 as string;


        }

    }
}
