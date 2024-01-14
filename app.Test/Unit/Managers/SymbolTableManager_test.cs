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
            testSymTable.RawSymbolTable.Add("test key", Engine.Types.NumType.NewInt(1));
            manager.UpdateSymbolTable(testSymTable.RawSymbolTable);

            // ----
            // ACT
            // ----
            SymbolTable actualSymTable = manager.GetSymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(actualSymTable.RawSymbolTable, Is.EqualTo(testSymTable.RawSymbolTable), "SymbolTables don't match");
        }

        [Test]
        public void Test_SymbolTableManager_UpdateSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------
            SymbolTableManager manager = new SymbolTableManager();
            
            SymbolTable testSymTable = new SymbolTable();
            testSymTable.RawSymbolTable.Add("test key", Engine.Types.NumType.NewInt(1));

            // ----
            // ACT
            // ----
            manager.UpdateSymbolTable(testSymTable.RawSymbolTable);
            SymbolTable afterUpdateSymTable = manager.GetSymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(afterUpdateSymTable.RawSymbolTable, Is.EqualTo(testSymTable.RawSymbolTable), "SymbolTables don't match");
        }

        [Test]
        public void Test_SymbolTableManager_ClearSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------
            SymbolTableManager manager = new SymbolTableManager();

            SymbolTable testSymTable = new SymbolTable();
            testSymTable.RawSymbolTable.Add("test key", Engine.Types.NumType.NewInt(1));
            manager.UpdateSymbolTable(testSymTable.RawSymbolTable);

            // ----
            // ACT
            // ----
            manager.ClearSymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(manager.GetSymbolTable().RawSymbolTable, Is.Empty, "SymbolTable must be empty");
        }
    }
}
