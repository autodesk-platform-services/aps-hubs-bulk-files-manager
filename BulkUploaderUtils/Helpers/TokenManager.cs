using ApsSettings.Data;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Data.Models;
using Flurl;
using mass_upload_via_s3_csharp;
using Serilog;
using System.Runtime.InteropServices;
using Autodesk.Forge.Api;
using BulkUploaderUtils;

namespace Data.Managers
{
    public static class TokenManager
    {
        private static string TwoLeggedToken { get; set; }
        private static DateTime TwoLeggedTokenExpiration { get; set; }
        private static readonly TwoLeggedApiV2 TwoLeggedApi = new TwoLeggedApiV2();
        
        public static async Task<string> GetTwoLeggedToken()
        {
            // if (!AppSettings.ConfigIsBuilt )
            //     AppSettings.BuildConfig();

            if (TwoLeggedToken != null && TwoLeggedTokenExpiration > (DateTime.UtcNow.AddMinutes(15)))
            {
                return TwoLeggedToken;
            }
            else
            {
                ApiResponse<dynamic> bearer = TwoLeggedApi.AuthenticateWithHttpInfo (AppSettings.Instance.ForgeClientId, 
                    AppSettings.Instance.ForgeClientSecret, oAuthConstants.CLIENT_CREDENTIALS, 
                    ScopeStringToArray(AppSettings.Instance.ForgeTwoLegScope)) ;
                if ( bearer.StatusCode != 200 )
                    throw new Exception ("Request failed! (with HTTP response " + bearer.StatusCode + ")") ;

                TwoLeggedToken = bearer.Data.access_token;
                TwoLeggedTokenExpiration = DateTime.UtcNow.AddSeconds(bearer.Data.expires_in);

                return TwoLeggedToken;
            }
        }
        
        
        public static Scope[] ScopeStringToArray(string scopeString)
        {
            var scopeStrings = scopeString.Split(' ').ToList();
            var scopes = new List<Scope>();

            if (scopeStrings.Contains("data:read")) scopes.Add(Scope.DataRead);
            if (scopeStrings.Contains("data:write")) scopes.Add(Scope.DataWrite);
            if (scopeStrings.Contains("data:create")) scopes.Add(Scope.DataCreate);
            if (scopeStrings.Contains("data:search")) scopes.Add(Scope.DataSearch);
            
            if (scopeStrings.Contains("account:read")) scopes.Add(Scope.AccountRead);
            if (scopeStrings.Contains("account:write")) scopes.Add(Scope.AccountWrite);
            
            if (scopeStrings.Contains("user:profileRead")) scopes.Add(Scope.UserProfileRead);
            
            if (scopeStrings.Contains("bucket:read")) scopes.Add(Scope.BucketRead);
            if (scopeStrings.Contains("bucket:update")) scopes.Add(Scope.BucketUpdate);
            if (scopeStrings.Contains("bucket:create")) scopes.Add(Scope.BucketCreate);
            if (scopeStrings.Contains("bucket:delete")) scopes.Add(Scope.BucketDelete);
            
            if (scopeStrings.Contains("code:all")) scopes.Add(Scope.CodeAll);
            
            return scopes.ToArray();
        }
    }
}