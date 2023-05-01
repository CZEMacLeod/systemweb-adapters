// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;

namespace Microsoft.AspNetCore.SystemWebAdapters.Hosting;

public class DefaultHostingEnvironment : ISystemWebHostingEnvironment
{
    public DefaultHostingEnvironment(IEnumerable<VirtualPathProvider> virtualPathProviders)
    {
        foreach (var vpp in virtualPathProviders.OfType<VirtualPathProvider>())
        {
            RegisterVirtualPathProvider(vpp);
        }
    }

    private VirtualPathProvider? virtualPathProvider;

    public VirtualPathProvider? VirtualPathProvider => virtualPathProvider;

    public void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
    {
        ArgumentNullException.ThrowIfNull(virtualPathProvider);

        VirtualPathProvider? previous = this.virtualPathProvider;
        this.virtualPathProvider = virtualPathProvider;

        // Give it the previous provider so it can delegate if needed
        virtualPathProvider.Initialize(previous);
    }
}
