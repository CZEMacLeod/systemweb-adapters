// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.SystemWebAdapters;

namespace System.Web;

public sealed class HttpRuntime
{
    private static IHttpRuntime? _current;

    internal static IHttpRuntime Current
    {
        get => _current ?? throw new InvalidOperationException("HttpRuntime is not available in the current environment");
        set => _current = value;
    }

    private HttpRuntime()
    {
    }

    public static string? AppDomainAppVirtualPath => Current.AppDomainAppVirtualPath;
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
    public static string AppDomainAppPath => Current.AppDomainAppPath ?? throw new ArgumentNullException("path");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

    public static string? AppDomainAppId => Current.AppDomainAppId;
    public static string? AppDomainId => Current.AppDomainId ?? string.Empty;
    public static Version? IISVersion => Current.IISVersion;
    public static Version TargetFramework => Current.TargetFramework;
}
