// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class ContentRootMapPath : AppPathMapPathBase
{
    private readonly IWebHostEnvironment _environment;

    public ContentRootMapPath(IWebHostEnvironment environment, IHttpContextAccessor context) : base(context) => _environment = environment;

    protected override string AppPath => _environment.ContentRootPath;
}
