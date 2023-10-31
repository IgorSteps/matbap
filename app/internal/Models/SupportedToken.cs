using System;
namespace app
{
	public class SupportedToken
	{
		private string _token;

		public SupportedToken(string token)
		{
			_token = token;
		}

		public string Token
		{
			get { return _token; }
		}
	}
}

