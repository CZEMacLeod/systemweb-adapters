// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web;

public sealed class HttpApplicationPool
{
    private HttpApplicationPool()
    {
    }

    public static string AppPoolId => Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process) ?? string.Empty;
    public static string AppPoolConfig => Environment.GetEnvironmentVariable("APP_POOL_CONFIG", EnvironmentVariableTarget.Process) ?? string.Empty;

#if NET6_0_OR_GREATER
    public static string? InstanceID => HttpRuntime.Current.InstanceID;
    public static string? InstanceMetaPath => HttpRuntime.Current.InstanceMetaPath;
#endif
#if NET40_OR_GREATER
    private static string? GetServerVariable(string variable) => HttpContext.Current?.Request?.ServerVariables[variable];
    public static string? InstanceID => GetServerVariable("INSTANCE_ID");
    public static string? InstanceMetaPath => GetServerVariable("INSTANCE_META_PATH");
#endif
}
