using System;
namespace app
{
	public class TokenHelp
	{
		private char _token;

		public TokenHelp(char token)
		{
			_token = token;
		}

		public string Token
		{
			get;
			set;
		}
	}
}

