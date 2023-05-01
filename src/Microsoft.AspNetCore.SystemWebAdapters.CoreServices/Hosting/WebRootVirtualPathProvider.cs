// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.SystemWebAdapters.Hosting;

public class WebRootVirtualPathProvider : FileProviderVirtualPathProvider
{
    public WebRootVirtualPathProvider(IWebHostEnvironment host) :
        base(host?.WebRootFileProvider ?? new NullFileProvider())
    { }
}
