using Autodesk.Authentication.Model;
using System.Collections.Generic;

namespace Ac.Net.Authentication.Models
{
    public interface IAuthParamProvider
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string RefreshToken { get; set; }
        List<Scopes> ForgeTwoLegScope { get; }
        List<Scopes> ForgeThreeLegScope { get; }
    }    
}
