using Autodesk.Authentication;
using Autodesk.DataManagement;
using Autodesk.Oss.Http;
using Autodesk.SDKManager;

namespace Bulk_Uploader_Electron.Helpers
{
    internal class APSClientHelper
    {
        internal static SDKManager SdkManager => SdkManagerBuilder.Create().Build();
        internal static AuthenticationClient AuthClient => new (SdkManager);
        internal static DataManagementClient DataManagement => new(SdkManager);
        internal static OSSApi OssApi => new(SdkManager);
    }
}
