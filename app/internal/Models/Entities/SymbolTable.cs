using System.Collections.Generic;

namespace app
{
    public class SymbolTable
    {
        public Dictionary<string, Engine.Types.NumType> RawSymbolTable { get; private set; }

        public SymbolTable()
        {
            RawSymbolTable = new Dictionary<string, Engine.Types.NumType>();
        }

        public void UpdateTable(Dictionary<string, Engine.Types.NumType> newTable)
        {
            RawSymbolTable = newTable;
        }
    }
}
