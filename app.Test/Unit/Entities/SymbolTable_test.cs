using RawSymTable = System.Collections.Generic.Dictionary<string, Engine.Types.NumType>;

namespace app.Test.Unit
{
    public class SymbolTable_test
    {
        [Test]
        public void Test_SymbolTable_CreateSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------

            // ----
            // ACT
            // ----
            SymbolTable testSymTable = new SymbolTable();

            // ------
            // ASSERT
            // ------
            Assert.That(testSymTable, Is.Not.Null, "SymbolTables can't be null");
        }

        [Test]
        public void Test_SymbolTable_UpdateSymbolTable()
        {
            // --------
            // ASSEMBLE
            // --------
            SymbolTable testSymTable = new SymbolTable();
            RawSymTable rawTable = new RawSymTable()
            {
                { "test key", Engine.Types.NumType.NewInt(1) }
            };

            // ----
            // ACT
            // ----
            testSymTable.UpdateTable(rawTable);

            // ------
            // ASSERT
            // ------
            Assert.That(testSymTable.RawSymbolTable, Is.EqualTo(rawTable), "SymbolTables must be the same");
        }
    }
}
