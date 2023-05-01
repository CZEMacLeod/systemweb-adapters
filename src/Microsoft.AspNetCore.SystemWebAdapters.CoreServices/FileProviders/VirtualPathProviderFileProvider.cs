// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;
using Microsoft.AspNetCore.SystemWebAdapters.Hosting;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.FileProviders;

public class VirtualPathProviderFileProvider : IFileProvider
{
    private readonly VirtualPathProvider virtualPathProvider;
    private readonly List<string> protectedPaths = new();

    public VirtualPathProviderFileProvider(VirtualPathProvider virtualPathProvider)
    {
        this.virtualPathProvider = virtualPathProvider;
        protectedPaths.Add("~/App_Data");
    }

    public VirtualPathProvider VirtualPathProvider => virtualPathProvider;

    private bool IsPathProtected(string virtualPath)
    {
        String checkPath = System.Web.VirtualPathUtility.ToAppRelative(virtualPath);
        return protectedPaths.Any(protectedPath => checkPath.StartsWith(protectedPath, StringComparison.InvariantCultureIgnoreCase));
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        if (IsPathProtected(subpath)) return NotFoundDirectoryContents.Singleton;
        var directory = VirtualPathProvider?.GetDirectory(subpath);
        if (directory is null) return NotFoundDirectoryContents.Singleton;
        return directory is FileProviderVirtualPathProvider.FileProviderVirtualDirectory vd ?
            vd.DirectoryContents :
            new VirtualDirectory.DirectoryContents(directory);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (IsPathProtected(subpath)) return new NotFoundFileInfo(subpath);
        if (VirtualPathProvider?.DirectoryExists(subpath) ?? false)
        {
            var directory = VirtualPathProvider?.GetDirectory(subpath);
            if (directory is not null)
            {
                return directory is FileProviderVirtualPathProvider.FileProviderVirtualDirectory vd ?
                    (vd.DirectoryContents is IFileInfo fi ? fi : vd.VirtualPathProvider.FileProvider.GetFileInfo(subpath)) :
                    new VirtualDirectory.DirectoryContents(directory);
            }
        }
        if (!(VirtualPathProvider?.FileExists(subpath) ?? false)) return new NotFoundFileInfo(subpath);

        var file = VirtualPathProvider?.GetFile(subpath);
        if (file is null) return new NotFoundFileInfo(subpath);

        return file is FileProviderVirtualPathProvider.FileProviderVirtualFile vf ?
            vf.FileInfo :
            new VirtualFile.FileInfo(file);
    }

    public IChangeToken Watch(string filter)
    {
        var cd = VirtualPathProvider?.GetCacheDependency(filter, Enumerable.Empty<string>(), DateTime.UtcNow);
        if (cd is null) return NullChangeToken.Singleton;
        return new CacheDependencyChangeToken(cd);
        //while (vpp is not null)
        //{
        //    if (vpp is FileProviderVirtualPathProvider fp)
        //    {
        //        return fp.FileProvider.Watch(filter);
        //    }
        //    vpp = vpp.Previous;
        //}
        //throw new NotImplementedException();
    }

    internal class CacheDependencyChangeToken : IChangeToken
    {
        private CacheDependency cacheDependency;

        public CacheDependencyChangeToken(CacheDependency cacheDependency)
        {
            ArgumentNullException.ThrowIfNull(cacheDependency);

            this.cacheDependency = cacheDependency;
            cacheDependency.SetCacheDependencyChanged((_, _) =>
            {
                foreach (var cb in callbacks)
                {
                    cb.Invoke();
                }
            });
        }

        public bool ActiveChangeCallbacks { get; private set; }
        public bool HasChanged => cacheDependency.HasChanged;

        private readonly List<Callback> callbacks = new();

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            ActiveChangeCallbacks = true;
            var cb = new Callback(callback, state, this);
            callbacks.Add(cb);
            return cb;
        }

        private class Callback : IDisposable
        {
            private Action<object>? callback;
            private object? state;
            private WeakReference<CacheDependencyChangeToken>? cacheDependencyChangeToken;
            private bool disposedValue;

            public Callback(Action<object> callback, object state, CacheDependencyChangeToken cacheDependencyChangeToken)
            {
                this.callback = callback;
                this.state = state;
                this.cacheDependencyChangeToken = new WeakReference<CacheDependencyChangeToken>(cacheDependencyChangeToken);
            }

#pragma warning disable CS8604 // Possible null reference argument.
            public void Invoke() => callback?.Invoke(state);
#pragma warning restore CS8604 // Possible null reference argument.

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        callback = null;
                        state = null;
                        if (cacheDependencyChangeToken is not null && cacheDependencyChangeToken.TryGetTarget(out var ct))
                        {
                            ct.callbacks.Remove(this);
                            cacheDependencyChangeToken = null;
                        }
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
