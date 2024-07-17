using Ac.Net.Authentication.Models;

namespace BulkUploaderUtils;

public class AppSettings : IAuthParamProvider
{
    public readonly static AppSettings Instance = new AppSettings();
    public string ForgeClientId { get; set; } = "";
    public string ForgeClientSecret { get; set; } = "";
    public string ForgeAuthCallback { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string ForgeTwoLegScope { get; set; } = "data:read data:write data:create bucket:read";
    public string ForgeThreeLegScope { get; set; } = "data:read data:write data:create bucket:read";
    public string FileWorkerCount { get; set; } = "10";
    public string FolderWorkerCount { get; set; } = "10";
    public string AccHubId { get; set; } = "10";
    public string ProjectId { get; set; } = "";
    public string ParentFolderUrn { get; set; } = "";
    public string LocalParentPath { get; set; } = "";

    public string OutputFolder { get; set; } = "";

    public string AccountID = "";

    public string[] IllegalFileTypes { get; set; } = new string[] { "ade", "adp","app","asp","bas","bat","cer","chm","class",
    "cmd","com","command","cpl","crt","csh","exe","fxp","hex","hlp","hqx","hta","htm","html","inf","ini","ins","isp","its","jar",
    "job","js","jse","ksh","lnk","lzh","mad","maf","mag","mam","maq","mar","mas","mau","mav","maw","mda","mde","mdt","mdw","mdz",
    "msc","msi","msp","mst","ocx","ops","pcd","pkg","pif","prf","prg","ps1","pst","reg","scf","scr","sct","sea","sh","shb","shs",
    "svg","tmp","url","vb","vbe","vbs","vsmacros","vss","vst","vsw","webloc","ws","wsc","wsf","wsh","zlo","zoo"};

    public string CustomerExcludedFileTypes { get; set; } = "";
    public string CustomerExcludedFolderNames { get; set; } = "";

    public bool ConfigIsBuilt = false;

    // public static void BuildConfig()
    // {
    //     // Create Configuration from Environment and AppSettings
    //     var configuration = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //         .AddJsonFile($"appsettings.json", true)
    //         .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    //         .AddJsonFile($"appsettings.Local.json", true)
    //         .AddEnvironmentVariables();

    //     var config = configuration.Build();

    //     AppSettings.ForgeClientId = config.GetValue<string>("AUTODESK_CLIENT_ID");
    //     AppSettings.ForgeClientSecret = config.GetValue<string>("AUTODESK_CLIENT_SECRET");
    //     AppSettings.ForgeTwoLegScope = config.GetValue<string>("AUTODESK_TWO_LEGGED_SCOPES", "data:read data:write data:create");
    //     AppSettings.FileWorkerCount = config.GetValue("FILE_WORKER_COUNT", "10");
    //     AppSettings.FolderWorkerCount = config.GetValue("FOLDER_WORKER_COUNT", "10");


    //     AppSettings.ProjectId = config.GetValue<string>("PROJECT_ID");
    //     AppSettings.LocalParentPath = config.GetValue<string>("LOCAL_PARENT_PATH");
    //     AppSettings.ParentFolderUrn = config.GetValue<string>("PARENT_FOLDER_URN");
    //     AppSettings.CustomerExcludedFileTypes = config.GetValue<string>("EXCLUDED_FILE_TYPES");
    //     AppSettings.CustomerExcludedFolderNames = config.GetValue<string>("EXCLUDED_FOLDER_NAMES");

    //     ConfigIsBuilt = true;
    // }

}
