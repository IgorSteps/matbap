using System.Collections.Generic;

namespace app
{
    public interface ISymbolTableManager
    {
        public void UpdateSymbolTable(Dictionary<string, Engine.Parser.NumType> newTable);
        public SymbolTable GetSymbolTable();
    }
}
