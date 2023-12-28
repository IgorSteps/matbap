using System.Collections.Generic;

namespace app
{
    public class SymbolTable
    {
        public Dictionary<string, Engine.Parser.NumType> Table { get; private set; }

        public SymbolTable()
        {
            Table = new Dictionary<string, Engine.Parser.NumType>();
        }

        public void UpdateTable(Dictionary<string, Engine.Parser.NumType> newTable)
        {
            Table = newTable;
        }
    }
}
