// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public abstract class AppPathMapPathBase : MapPathBase, IMapPath
{
    protected AppPathMapPathBase(IHttpContextAccessor context) : base(context) { }
    protected abstract string AppPath { get; }

    public string MapPath(string? virtualPath)
    {
        var relPath = RelPath(virtualPath);
        if (string.IsNullOrEmpty(relPath)) return AppPath;
        return System.IO.Path.Combine(AppPath,
            relPath[1..]
            .Replace('/', System.IO.Path.DirectorySeparatorChar))
            .TrimEnd(System.IO.Path.DirectorySeparatorChar);
    }
}
