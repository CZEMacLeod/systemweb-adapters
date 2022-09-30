// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.SystemWebAdapters.Hosting;

namespace System.Web.Hosting;

public abstract class VirtualPathProvider : IVirtualPathProvider
{
    private VirtualPathProvider? _previous;

    #region "Construction and Initialization"
    protected VirtualPathProvider()
    {

    }

    internal virtual void Initialize(VirtualPathProvider? previous)
    {
        _previous = previous;
        Initialize();
    }
    protected virtual void Initialize() { }

    //public override object InitializeLifetimeService();
    #endregion

    #region "Utility Methods"
    public virtual string CombineVirtualPaths(string basePath, string relativePath) => VirtualPathUtility.Combine(basePath, relativePath);

    public static System.IO.Stream? OpenFile(string virtualPath)
    {
        VirtualPathProvider? vpathProvider = System.Web.Hosting.HostingEnvironment.VirtualPathProvider;
        VirtualFile? vfile = vpathProvider?.GetFile(virtualPath);
        if (vfile == null) return null;
        if (!string.Equals(virtualPath, vfile.VirtualPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new HttpException($"Bad VirtualPath in VirtualFileBase: {virtualPath} != {vfile.VirtualPath}");
        }
        return vfile?.Open();
    }
    #endregion

    #region "Properties"
    protected internal VirtualPathProvider? Previous => _previous;
    #endregion

    #region "Default implementation delegates to previous provider"
    public virtual bool DirectoryExists(string virtualDir) => _previous is null ? false : _previous.DirectoryExists(virtualDir);

    public virtual bool FileExists(string virtualPath) => _previous is null ? false : _previous.FileExists(virtualPath);

    public virtual System.Web.Caching.CacheDependency? GetCacheDependency(string virtualPath,
                                                                         System.Collections.IEnumerable virtualPathDependencies,
                                                                         DateTime utcStart) =>
        _previous?.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);

    public virtual string? GetCacheKey(string virtualPath) => null;

    public virtual VirtualDirectory? GetDirectory(string virtualDir) => _previous?.GetDirectory(virtualDir);

    public virtual VirtualFile? GetFile(string virtualPath) => _previous?.GetFile(virtualPath);

    public virtual string? GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies) =>
        _previous?.GetFileHash(virtualPath, virtualPathDependencies);
    #endregion
}
