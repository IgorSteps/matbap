using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public interface IInterpretModel
    {
        public string Interpret(string expression);
    }
}
