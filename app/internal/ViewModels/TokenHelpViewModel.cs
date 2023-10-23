using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

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
                new TokenHelp('-'),
                new TokenHelp('*'),
                new TokenHelp('/'),
                new TokenHelp('%'),
                new TokenHelp('^'),
                new TokenHelp('('),
                new TokenHelp(')'),
                new TokenHelp('.'),
            };
        }
	}
}

