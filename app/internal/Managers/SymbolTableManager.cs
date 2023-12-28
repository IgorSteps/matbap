using System.Collections.Generic;

namespace app
{
    public class SymbolTableManager : ISymbolTableManager
    {
        public SymbolTable Table { get; private set; }
        
        public SymbolTableManager() 
        {
            Table = new SymbolTable();
        }

        public SymbolTable GetSymbolTable()
        {
            return Table;
        }

        public void UpdateSymbolTable(Dictionary<string, Engine.Parser.NumType> newTable)
        {
            Table.UpdateTable(newTable);
        }
    }
}
