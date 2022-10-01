// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web.Hosting;

namespace Microsoft.AspNetCore.SystemWebAdapters.Hosting;

public interface IHostingEnvironmentAdapter
{
    VirtualPathProvider? VirtualPathProvider { get; }
    void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider);

    System.Web.Caching.Cache Cache { get; }
}
