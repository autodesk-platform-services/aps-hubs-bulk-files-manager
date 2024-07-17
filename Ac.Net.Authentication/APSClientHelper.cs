using Autodesk.Authentication;
using Autodesk.SDKManager;

namespace Ac.Net.Authentication
{
    internal class APSClientHelper
    {
        internal static SDKManager SdkManager => SdkManagerBuilder.Create().Build();
        internal static AuthenticationClient AuthClient => new AuthenticationClient(SdkManager);
    }
}
