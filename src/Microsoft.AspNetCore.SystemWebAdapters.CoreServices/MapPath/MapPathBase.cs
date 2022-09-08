// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public abstract class MapPathBase
{
    private readonly IHttpContextAccessor _context;

    protected MapPathBase(IHttpContextAccessor context) => _context = context;

    protected string? RelPath(string? virtualPath) => string.IsNullOrEmpty(virtualPath) ?
        VirtualPathUtility.GetDirectory(_context.HttpContext.Request.Path) :
        VirtualPathUtility.Combine(
                VirtualPathUtility.GetDirectory(_context.HttpContext.Request.Path) ?? "/"
                , virtualPath);
}
