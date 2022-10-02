// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Partially based on code in https://github.com/microsoft/referencesource/blob/master/System.Web/Hosting/HostingEnvironment.cs
// to ensure compatibility with code bypassing the BuildManager.IsPrecompiledApp check

using Microsoft.AspNetCore.SystemWebAdapters.Hosting;

namespace System.Web.Hosting;

public sealed class HostingEnvironment
{
    private static ISystemWebHostingEnvironment? _current;

    internal static ISystemWebHostingEnvironment Current
    {
        get => _current ?? throw new InvalidOperationException("HostingEnvironment is not available in the current environment");
        set => _current = value;
    }

    public static System.Web.Hosting.VirtualPathProvider? VirtualPathProvider => Current?.VirtualPathProvider;

    public static void RegisterVirtualPathProvider(System.Web.Hosting.VirtualPathProvider virtualPathProvider)
    {
        RegisterVirtualPathProviderInternal(virtualPathProvider);
    }

    internal static void RegisterVirtualPathProviderInternal(VirtualPathProvider virtualPathProvider)
    {
        if (Current is null)
        {
            throw new InvalidOperationException();
        }

        Current.RegisterVirtualPathProvider(virtualPathProvider);
    }

    #region "Alternative access via HttpRuntime"
    public static System.Web.Caching.Cache Cache => HttpRuntime.Cache;

    public static string ApplicationPhysicalPath => HttpRuntime.AppDomainAppPath;

    public static string ApplicationVirtualPath => HttpRuntime.AppDomainAppVirtualPath;
    #endregion
}
