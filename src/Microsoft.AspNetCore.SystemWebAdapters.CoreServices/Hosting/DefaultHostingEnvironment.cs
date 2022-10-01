// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;

namespace Microsoft.AspNetCore.SystemWebAdapters.Hosting;

public class DefaultHostingEnvironment : IHostingEnvironmentAdapter
{
    public DefaultHostingEnvironment(ISystemWebCacheFactory cacheFactory, IEnumerable<IVirtualPathProvider> virtualPathProviders)
    {
        this.cacheFactory = cacheFactory;
        foreach (var vpp in virtualPathProviders.OfType<VirtualPathProvider>())
        {
            RegisterVirtualPathProvider(vpp);
        }
    }

    private VirtualPathProvider? virtualPathProvider;
    private Cache? cache;
    private readonly ISystemWebCacheFactory cacheFactory;

    public VirtualPathProvider? VirtualPathProvider => virtualPathProvider;

    public Cache Cache => cache ??= new Cache(cacheFactory.GetCache());

    public void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
    {
        VirtualPathProvider? previous = this.virtualPathProvider;
        this.virtualPathProvider = virtualPathProvider;

        // Give it the previous provider so it can delegate if needed
        virtualPathProvider.Initialize(previous);
    }
}
