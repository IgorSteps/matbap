namespace app.Test.Unit
{
    public class SymbolTableManager_test
    {
        [Test]
        public void Test_SymbolTableManager_GetSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------
            SymbolTableManager manager = new SymbolTableManager();
            SymbolTable testSymTable = new SymbolTable();
            testSymTable.Table.Add("test key", Engine.Types.NumType.NewInt(1));
            manager.UpdateSymbolTable(testSymTable.Table);

            // ----
            // ACT
            // ----
            SymbolTable actualSymTable = manager.GetSymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(actualSymTable.Table, Is.EqualTo(testSymTable.Table), "SymbolTables don't match");
        }

        [Test]
        public void Test_SymbolTableManager_UpdateSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------
            SymbolTableManager manager = new SymbolTableManager();
            
            SymbolTable testSymTable = new SymbolTable();
            testSymTable.Table.Add("test key", Engine.Types.NumType.NewInt(1));

            // ----
            // ACT
            // ----
            manager.UpdateSymbolTable(testSymTable.Table);
            SymbolTable afterUpdateSymTable = manager.GetSymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(afterUpdateSymTable.Table, Is.EqualTo(testSymTable.Table), "SymbolTables don't match");
        }
    }
}
