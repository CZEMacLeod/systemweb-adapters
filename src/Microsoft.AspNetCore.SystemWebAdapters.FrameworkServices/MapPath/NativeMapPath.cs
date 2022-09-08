// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class NativeMapPath : IMapPath
{
    private readonly HttpContext _context;
    private readonly HttpContextBase _contextBase;

    public NativeMapPath(HttpContext context) => _context = context;
    public NativeMapPath(HttpContextBase context) => _contextBase = context;

    public string MapPath(string? virtualPath) => _contextBase is not null ?
        _contextBase.Server.MapPath(virtualPath)
        : _context.Server.MapPath(virtualPath);
}

