using System.Collections.Generic;

namespace app
{
    public interface ISymbolTableManager
    {
        public void UpdateSymbolTable(Dictionary<string, Engine.Types.NumType> newTable);
        public void ClearSymbolTable();
        public SymbolTable GetSymbolTable();
    }
}
