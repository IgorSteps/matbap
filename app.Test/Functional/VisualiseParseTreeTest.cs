﻿namespace app.Test.Functional
{
    // @TODO: Functionality is not implemented yet.
    public class VisualiseParseTreeTest
    {
        /// <summary>
        /// Test user can visualuse a parse tree of the equation entered.
        /// </summary>
        [Test]
        public void TestVisualiseParseTree()
        {
            // --------
            // ASSEMBLE
            // --------
            ASTConversionService converter = new ASTConversionService();
            ASTViewModel viewModel = new ASTViewModel(converter);

            // -----
            // ACT
            // -----


            // ------
            // ASSERT
            // ------
        }
    }
}