using System.Collections.Generic;

namespace app
{
    public class SymbolTable
    {
        public Dictionary<string, Engine.Types.NumType> Table { get; private set; }

        public SymbolTable()
        {
            Table = new Dictionary<string, Engine.Types.NumType>();
        }

        public void UpdateTable(Dictionary<string, Engine.Types.NumType> newTable)
        {
            Table = newTable;
        }
    }
}
