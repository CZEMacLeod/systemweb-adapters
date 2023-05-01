// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection;

public static class VirtualPathProviderExtensions
{
    public static ISystemWebAdapterBuilder AddVirtualPathProvidersAsStaticFileProvider(this ISystemWebAdapterBuilder adapter, Action<StaticFileOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(adapter);

        adapter.Services.AddOptions<StaticFileOptions>().PostConfigure<ISystemWebHostingEnvironment>((options, env) =>
        {
            var vpp = env?.VirtualPathProvider;
            if (vpp is null) return;
            options.FileProvider = new VirtualPathProviderFileProvider(vpp);
            configure?.Invoke(options);
        });
        return adapter;
    }

    public static ISystemWebAdapterBuilder AddVirtualPathProvider(this ISystemWebAdapterBuilder adapter, VirtualPathProvider virtualPathProvider)
    {
        ArgumentNullException.ThrowIfNull(adapter);

        adapter.Services.AddSingleton<VirtualPathProvider>(_ => virtualPathProvider);
        return adapter;
    }

    public static ISystemWebAdapterBuilder AddVirtualPathProvider<T>(this ISystemWebAdapterBuilder adapter)
        where T : VirtualPathProvider
    {
        ArgumentNullException.ThrowIfNull(adapter);

        adapter.Services.AddSingleton<VirtualPathProvider, T>();
        return adapter;
    }
}
