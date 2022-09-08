// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class NativeMapPath : IMapPath
{
    private readonly HttpContext _context;

    public NativeMapPath(HttpContext context) => _context = context;

    public string MapPath(string? virtualPath) => _context.Server.MapPath(virtualPath);
}

