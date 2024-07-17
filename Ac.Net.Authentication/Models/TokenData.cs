using System;

namespace Ac.Net.Authentication.Models
{
    public class TokenData
    {
        public readonly string access_token;

        public readonly DateTime expiresAt;

        public readonly string refresh_token;

        public TokenData(string access_token, string refresh_token, DateTime expiresAt)
        {
            this.access_token = access_token;
            this.refresh_token = refresh_token;
            this.expiresAt = expiresAt;
        }
        public bool isValid
        {
            get
            {
                return DateTime.Now < expiresAt;
            }
        }
    }
}