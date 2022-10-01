// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using Microsoft.Extensions.FileProviders;
using System.Web.Hosting;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.Linq;

namespace Microsoft.AspNetCore.SystemWebAdapters.Hosting;

// This actually works on Framework too, but requires a <PackageReference Include="Microsoft.Extensions.FileProviders.Abstractions" Version="6.0.0" />
public class FileProviderVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
{
    private readonly IFileProvider fileProvider;

    public IFileProvider FileProvider => fileProvider;

    public FileProviderVirtualPathProvider(IFileProvider fileProvider)
    {
        this.fileProvider = fileProvider;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    private string VirtualPath(string virtualPath) =>
        VirtualPathUtility.IsAppRelative(virtualPath) ?
            VirtualPathUtility.ToAbsolute(virtualPath, "/") :
            virtualPath;

    public override bool DirectoryExists(string virtualDir)
    {
        var dc = FileProvider.GetDirectoryContents(VirtualPath(virtualDir));
        if (dc is not null && dc.Exists) return true;
        return base.DirectoryExists(virtualDir);
    }

    public override bool FileExists(string virtualPath)
    {
        var fi = FileProvider.GetFileInfo(VirtualPath(virtualPath));
        if (fi is not null && fi.Exists && !fi.IsDirectory) return true;
        return base.FileExists(virtualPath);
    }

    public override CacheDependency? GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
    {
        var fi = FileProvider.GetFileInfo(VirtualPath(virtualPath));
        if (fi is not null && fi.Exists)
        {
            return new ChangeTokenCacheDependency(FileProvider.Watch(virtualPath), fi.LastModified);
        }

        try
        {
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
        catch (HttpException) // when (ex.ErrorCode == -2147024893 || ex.ErrorCode == -2147024894)
        {
            return null;
        }
    }

    public override VirtualDirectory? GetDirectory(string virtualDir)
    {
        var dc = FileProvider.GetDirectoryContents(VirtualPath(virtualDir));
        if (dc is not null && dc.Exists) return new FileProviderVirtualDirectory(dc, virtualDir, this);
        return base.GetDirectory(virtualDir);
    }

    public override VirtualFile? GetFile(string virtualPath)
    {
        var fi = FileProvider.GetFileInfo(VirtualPath(virtualPath));
        if (fi is not null && fi.Exists && !fi.IsDirectory) return new FileProviderVirtualFile(fi, virtualPath);
        return base.GetFile(virtualPath);
    }

    public override string? GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
    {
        var fi = FileProvider.GetFileInfo(VirtualPath(virtualPath));
        if (fi is not null && fi.Exists && !fi.IsDirectory)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            using var fis = fi.CreateReadStream();
            var hash = md5.ComputeHash(fis);
            return Convert.ToBase64String(hash);
        }
        return base.GetFileHash(virtualPath, virtualPathDependencies);
    }

    internal class FileProviderVirtualDirectory : VirtualDirectory
    {
        private readonly IDirectoryContents directoryContents;
        private readonly FileProviderVirtualPathProvider vpp;

        public FileProviderVirtualDirectory(IDirectoryContents directoryContents,
                                            string virtualDir,
                                            FileProviderVirtualPathProvider vpp) : base(virtualDir)
        {
            this.directoryContents = directoryContents;
            this.vpp = vpp;
        }

        private VirtualFileBase? ChildItem(IFileInfo fileInfo) =>
            fileInfo.IsDirectory ?
            VirtualPathProvider.GetDirectory(VirtualPathProvider.CombineVirtualPaths(VirtualPath, fileInfo.Name)) :
            new FileProviderVirtualFile(fileInfo, VirtualPathProvider.CombineVirtualPaths(VirtualPath, fileInfo.Name));

        private VirtualFile FileItem(IFileInfo fileInfo) =>
            new FileProviderVirtualFile(fileInfo, VirtualPathProvider.CombineVirtualPaths(VirtualPath, fileInfo.Name));

        private VirtualDirectory? DirectoryItem(IFileInfo fileInfo) =>
            VirtualPathProvider.GetDirectory(VirtualPathProvider.CombineVirtualPaths(VirtualPath, fileInfo.Name));

        public override IEnumerable Directories => DirectoryContents.Where(fi => fi.IsDirectory).Select(DirectoryItem);

        public override IEnumerable Files => DirectoryContents.Where(fi => !fi.IsDirectory).Select(FileItem);

        public override IEnumerable Children => DirectoryContents.Select(ChildItem);

        new public IDirectoryContents DirectoryContents => directoryContents;

        public FileProviderVirtualPathProvider VirtualPathProvider => vpp;
    }

    internal class FileProviderVirtualFile : VirtualFile
    {
        private readonly IFileInfo fileInfo;

        public FileProviderVirtualFile(IFileInfo fileInfo, string virtualPath) : base(virtualPath) => this.fileInfo = fileInfo;

        new public IFileInfo FileInfo => fileInfo;

        public override Stream Open() => FileInfo.CreateReadStream();
    }
}
