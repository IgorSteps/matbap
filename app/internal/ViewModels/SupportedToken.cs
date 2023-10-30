using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace app
{
	public class TokenHelpViewModel : ObservableObject
    {
        private List<SupportedToken> _supportedTokens { get; set; }

        public TokenHelpViewModel()
		{
            _supportedTokens = new List<SupportedToken>
            {
                new SupportedToken("Plus +"),
                new SupportedToken("Minus -"),
                new SupportedToken("Unary Minus -5"),
                new SupportedToken("Unary Minus -(5+1)"),
                new SupportedToken("Multiply *"),
                new SupportedToken("Implicit multiplication 5x==5*x"),
                new SupportedToken("Divide /"),
                new SupportedToken("Remainder %"),
                new SupportedToken("Power ^"),
                new SupportedToken("Brackets ( or )"),
                new SupportedToken("Dot ."),
                new SupportedToken("Assignment ="),
                new SupportedToken("Trig functions sin,cos,tan"),
            };
        }

        public List<SupportedToken> SupportedTokens { get {  return _supportedTokens; } }
	}
}

