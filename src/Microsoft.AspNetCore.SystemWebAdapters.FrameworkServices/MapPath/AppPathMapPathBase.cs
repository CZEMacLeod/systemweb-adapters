// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public abstract class AppPathMapPathBase : MapPathBase, IMapPath
{
    protected AppPathMapPathBase(HttpContext context) : base(context) { }

    protected abstract string AppPath { get; }

    public string MapPath(string? virtualPath)
    {
        var relPath = RelPath(virtualPath);
        if (string.IsNullOrEmpty(relPath)) return AppPath;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return System.IO.Path.Combine(AppPath,
            relPath.Substring(1)
            .Replace('/', System.IO.Path.DirectorySeparatorChar))
            .TrimEnd(System.IO.Path.DirectorySeparatorChar);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}

public abstract class MapPathBase
{
    private readonly HttpContext _context;

    protected MapPathBase(HttpContext context) => _context = context;

    protected string? RelPath(string? virtualPath) => string.IsNullOrEmpty(virtualPath) ?
        VirtualPathUtility.GetDirectory(_context.Request.Path) :
        VirtualPathUtility.Combine(
            VirtualPathUtility.GetDirectory(_context.Request.Path) ?? "/"
            , virtualPath);
}
