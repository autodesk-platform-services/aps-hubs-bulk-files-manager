using Ac.Net.Authentication.Models;
using Autodesk.Authentication.Model;
using System.Collections.Concurrent;

namespace Bulk_Uploader_Electron;

public class AppSettings : IAuthParamProvider
{
    public ConcurrentDictionary<string, string> GlobalSettings { get; } = new ConcurrentDictionary<string, string>();

    public readonly static AppSettings Instance = new();
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public List<Scopes> ForgeTwoLegScope => new() { Scopes.DataRead, Scopes.DataWrite, Scopes.DataCreate, Scopes.BucketRead};
    public List<Scopes> ForgeThreeLegScope => new() { Scopes.DataRead, Scopes.DataWrite, Scopes.DataCreate, Scopes.BucketRead};
    public string FileWorkerCount { get; set; } = "10";
    public string FolderWorkerCount { get; set; } = "10";

    public string AccountID = "";

    public string[] IllegalFileTypes { get; set; } = new string[] { "ade", "adp","app","asp","bas","bat","cer","chm","class",
    "cmd","com","command","cpl","crt","csh","exe","fxp","hex","hlp","hqx","hta","htm","html","inf","ini","ins","isp","its","jar",
    "job","js","jse","ksh","lnk","lzh","mad","maf","mag","mam","maq","mar","mas","mau","mav","maw","mda","mde","mdt","mdw","mdz",
    "msc","msi","msp","mst","ocx","ops","pcd","pkg","pif","prf","prg","ps1","pst","reg","scf","scr","sct","sea","sh","shb","shs",
    "svg","tmp","url","vb","vbe","vbs","vsmacros","vss","vst","vsw","webloc","ws","wsc","wsf","wsh","zlo","zoo"};

    public string CustomerExcludedFileTypes { get; set; } = "";
    public string CustomerExcludedFolderNames { get; set; } = "";

    public bool ConfigIsBuilt = false;

    public static void BuildConfig()
    {
        // Create Configuration from Environment and AppSettings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
            .AddJsonFile($"appsettings.Local.json", true)
            .AddEnvironmentVariables();
        _ = configuration.Build();
    }
}
