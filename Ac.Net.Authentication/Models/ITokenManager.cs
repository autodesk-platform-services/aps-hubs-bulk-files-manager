using System.Threading.Tasks;

namespace Ac.Net.Authentication.Models
{
    /// <summary>
    /// Deleglte for when a token is refreshed
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="newToken"></param>
    public delegate void TokenUpdate(ITokenManager manager, TokenData newToken);

    /// <summary>
    ///
    /// </summary>
    public interface ITokenManager
    {
        event TokenUpdate OnTokenUpdate;

        Task<string> GetToken();

        bool IsAuthenticated { get; }

        void Logout();
    }
}