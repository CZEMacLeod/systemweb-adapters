// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HostingEnvironmentFactory
{
    public static IHostingEnvironmentAdapter Create(IServiceProvider sp)
    {
        IHostingEnvironmentAdapter env = new DefaultHostingEnvironment();
        foreach(var vpp in sp.GetServices<IVirtualPathProvider>().OfType<VirtualPathProvider>())
        {
            env.RegisterVirtualPathProvider(vpp);
        }
        return env;
    }

    public static IVirtualPathProvider CreateContentRootVirtualPathProvider(IServiceProvider serviceProvider)
    {
        var webHost = serviceProvider.GetService<IWebHostEnvironment>();
        var fileProvider = webHost?.WebRootFileProvider ?? new FileProviders.NullFileProvider();
        var vpp = new FileProviderVirtualPathProvider(fileProvider);
        return vpp;
    }

    public static IVirtualPathProvider CreateWebRootVirtualPathProvider(IServiceProvider serviceProvider)
    {
        var webHost = serviceProvider.GetService<IWebHostEnvironment>();
        var fileProvider = webHost?.WebRootFileProvider ?? new FileProviders.NullFileProvider();
        var vpp = new FileProviderVirtualPathProvider(fileProvider);
        return vpp;
    }

    internal class DefaultHostingEnvironment : IHostingEnvironmentAdapter
    {
        private VirtualPathProvider? _virtualPathProvider;

        public VirtualPathProvider? VirtualPathProvider => _virtualPathProvider;
        public void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
        {
            VirtualPathProvider? previous = _virtualPathProvider;
            _virtualPathProvider = virtualPathProvider;

            // Give it the previous provider so it can delegate if needed
            virtualPathProvider.Initialize(previous);
        }
    }

}
