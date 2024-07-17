using Autodesk.Authentication.Model;
using Bulk_Uploader_Electron.Helpers;

namespace Bulk_Uploader_Electron.Managers
{
    public static class TwoLeggedTokenManager
    {
        #region Properties
        private static string TwoLeggedToken { get; set; } = string.Empty;
        private static DateTime TwoLeggedTokenExpiration { get; set; }
        private static string? ClientId { get; set; }
        private static string? ClientSecret { get; set; }
        #endregion


        #region Methods
        public static async Task<string> GetTwoLeggedToken()
        {
            if (ClientId != AppSettings.Instance.ClientId || ClientSecret != AppSettings.Instance.ClientSecret)
            {
                ClientId = AppSettings.Instance.ClientId;
                ClientSecret = AppSettings.Instance.ClientSecret;
                return await RequestTwoLeggedToken();
            }

            if (!string.IsNullOrEmpty(TwoLeggedToken) && TwoLeggedTokenExpiration > (DateTime.UtcNow.AddMinutes(15)))
                return TwoLeggedToken;
            else
                return await RequestTwoLeggedToken();
        }
        private static async Task<string> RequestTwoLeggedToken()
        { 
            var twoLeggedToken = await APSClientHelper.AuthClient.GetTwoLeggedTokenAsync(AppSettings.Instance.ClientId, AppSettings.Instance.ClientSecret, AppSettings.Instance.ForgeTwoLegScope);

            TwoLeggedToken = twoLeggedToken.AccessToken;
            TwoLeggedTokenExpiration = DateTime.UtcNow.AddSeconds(twoLeggedToken.ExpiresIn ?? 60);

            return TwoLeggedToken;
        }
        #endregion
    }
}
