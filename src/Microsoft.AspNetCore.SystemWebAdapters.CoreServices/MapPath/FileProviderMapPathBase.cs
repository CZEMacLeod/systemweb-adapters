// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public abstract class FileProviderMapPathBase : MapPathBase, IMapPath
{
    private readonly IFileProvider _fileProvider;

    protected FileProviderMapPathBase(IFileProvider fileProvider, IHttpContextAccessor context) : base(context) => _fileProvider = fileProvider;

    public string MapPath(string? virtualPath)
    {
        var relPath = RelPath(virtualPath)?.Replace('/', System.IO.Path.DirectorySeparatorChar); 
        var fileProvider = _fileProvider;
        if (_fileProvider is CompositeFileProvider composite)
        {
            foreach(var fp in composite.FileProviders)
            {
                var path = fp.GetFileInfo(relPath).PhysicalPath;
                if (!string.IsNullOrEmpty(path)) return path;
            }
        }
        return fileProvider.GetFileInfo(relPath).PhysicalPath;
    }
}
