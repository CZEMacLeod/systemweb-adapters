// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class WebRootFileProviderMapPath : FileProviderMapPathBase
{
    public WebRootFileProviderMapPath(IWebHostEnvironment environment, IHttpContextAccessor context) :
        base(environment.WebRootFileProvider, context)
    { }
}
