using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace app
{
	public class TokenHelpViewModel : ObservableObject
    {
        private List<TokenHelp> _supportedTokens { get; set; }

        public TokenHelpViewModel()
		{
            _supportedTokens = new List<TokenHelp>
            {
                new TokenHelp('+'),
                // TODO other tokens
            };
        }
	}
}

