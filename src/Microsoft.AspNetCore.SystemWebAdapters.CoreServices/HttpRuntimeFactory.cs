// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SystemWebAdapters;

namespace Microsoft.Extensions.DependencyInjection;

internal static class HttpRuntimeFactory
{

    public static IHttpRuntime Create(IHttpContextAccessor? httpContextAccessor)
    {
        if (NativeMethods.IsAspNetCoreModuleLoaded())
        {
            var config = NativeMethods.HttpGetApplicationProperties();

            return new IISHttpRuntime(config, httpContextAccessor);
        }

        return new DefaultHttpRuntime();
    }

    internal abstract class HttpRuntimeBase
    {
#pragma warning disable CA1822 // Mark members as static
        public Version TargetFramework
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                var name = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
                if (string.IsNullOrEmpty(name)) return new Version();
                var framework = new FrameworkName(name);
                return framework.Version;
            }
        }
#pragma warning restore CA1822 // Mark members as static
    }

    internal class DefaultHttpRuntime : HttpRuntimeBase, IHttpRuntime
    {
        public string AppDomainAppVirtualPath => "/";

        public string AppDomainAppPath => AppContext.BaseDirectory;

        public string? AppDomainAppId => null;
        public string? AppDomainId => null;
        public Version? IISVersion => null;

        public string? InstanceID => null;
        public string? InstanceMetaPath => null;

    }
    internal class IISHttpRuntime : HttpRuntimeBase, IHttpRuntime
    {
        private readonly NativeMethods.IISConfigurationData _config;
        private readonly IHttpContextAccessor? httpContextAccessor;

        public IISHttpRuntime(NativeMethods.IISConfigurationData config, IHttpContextAccessor? httpContextAccessor)
        {
            _config = config;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string AppDomainAppVirtualPath => _config.pwzVirtualApplicationPath;

        public string AppDomainAppPath => _config.pwzFullApplicationPath;

        private string? GetServerVariable(string variable) => httpContextAccessor?.HttpContext?.GetServerVariable(variable);

        private Version? GetIISVersion()
        {
            var version = GetServerVariable("SERVER_SOFTWARE")?.Split('/');
            if (version is null || version.Length != 2)
            {
                return null;
            }
            Debug.Assert(version[0] == "Microsoft-IIS");
            _ = Version.TryParse(version[1], out var result);
            return result;
        }

        public string? AppDomainAppId => GetServerVariable("APPL_MD_PATH");
        private string domainID = "-1-" + DateTime.UtcNow.ToFileTimeUtc().ToString("G", CultureInfo.InvariantCulture);
#pragma warning disable CA1822 // Mark members as static
        public string AppDomainId => AppDomainAppId + domainID;

        public Version? IISVersion => GetIISVersion();

        public string? InstanceID => GetServerVariable("INSTANCE_ID");
        public string? InstanceMetaPath => GetServerVariable("INSTANCE_META_PATH");
    }
}
