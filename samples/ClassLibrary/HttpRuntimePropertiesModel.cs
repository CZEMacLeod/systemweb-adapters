using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassLibrary;

public sealed class HttpRuntimePropertiesModel
{
    public string AppDomainAppId { get; set; } = HttpRuntime.AppDomainAppId;
    public string AppDomainId { get; set; } = HttpRuntime.AppDomainId;
    public Version IISVersion { get; set; } = HttpRuntime.IISVersion;
    public Version TargetFramework { get; set; } = HttpRuntime.TargetFramework;

    public string AppDomainAppPath { get; set; } = HttpRuntime.AppDomainAppPath;
    public string AppDomainAppVirtualPath { get; set; } = HttpRuntime.AppDomainAppVirtualPath;

    public string AppPoolId { get; set; } = HttpApplicationPool.AppPoolId;
    public string AppPoolConfig { get; set; } = HttpApplicationPool.AppPoolConfig;

    public string InstanceID { get; set; } = HttpApplicationPool.InstanceID;
    public string InstanceMetaPath { get; set; } = HttpApplicationPool.InstanceMetaPath;

    //public string SERVER_SOFTWARE { get; set; } = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];

    //public IEnumerable<KeyValuePair<string, string>> Environment { get; set; } = GetEnvironmentVariables()
    //    .Where(kv => EnvironmentFilter(kv));

    //public IEnumerable<KeyValuePair<string, string>> Environment_Other { get; set; } = GetEnvironmentVariables()
    //    .Where(kv => !EnvironmentFilter(kv));

    //public IEnumerable<KeyValuePair<string, string>> ServerVariables { get; set; } = GetServerVariables();

    //public static bool EnvironmentFilter(KeyValuePair<string, string> kv)
    //{
    //    if (kv.Key.StartsWith("APP_")) return true;
    //    if (kv.Key.StartsWith("ASPNETCORE_")) return true;
    //    return false;
    //}

    //public static IEnumerable<KeyValuePair<string, string>> GetEnvironmentVariables()
    //{
    //    var variables = System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
    //    foreach (var key in variables.Keys.Cast<string>().OrderBy(k => k))
    //    {
    //        yield return new KeyValuePair<string, string>(key, (string)variables[key]);
    //    }
    //}

    public static IEnumerable<KeyValuePair<string, string>> GetServerVariables()
    {
        var variables = HttpContext.Current.Request.ServerVariables;
        IList<string> keys;
        try
        {
            keys = variables.Keys.Cast<string>().OrderBy(k => k).ToList();
        }
        catch (Exception)
        {
            yield break;
        }

        foreach (var key in keys)
        {
            yield return new KeyValuePair<string, string>(key, (string)variables[key]);
        }
    }
}
