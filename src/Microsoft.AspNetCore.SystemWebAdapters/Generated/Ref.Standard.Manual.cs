// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web;

public enum SameSiteMode
{
    None = 0,
    Lax = 1,
    Strict = 2,
}
public static partial class HttpApplicationPool
{
    public static string AppPoolConfig { get { throw new System.PlatformNotSupportedException("Only supported when running on ASP.NET Core or System.Web"); } }
    public static string AppPoolId { get { throw new System.PlatformNotSupportedException("Only supported when running on ASP.NET Core or System.Web"); } }
    public static string InstanceID { get { throw new System.PlatformNotSupportedException("Only supported when running on ASP.NET Core or System.Web"); } }
    public static string InstanceMetaPath { get { throw new System.PlatformNotSupportedException("Only supported when running on ASP.NET Core or System.Web"); } }
}
