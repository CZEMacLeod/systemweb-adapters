// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Web.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.SystemWebAdapters;

public abstract class FileInfoBase<T> : IFileInfo
    where T : VirtualFileBase
{
    protected readonly T virtualFile;

    protected FileInfoBase(T virtualFile) => this.virtualFile = virtualFile;

    public bool Exists => true;
    public bool IsDirectory=> virtualFile.IsDirectory;
    public virtual DateTimeOffset LastModified => DateTimeOffset.UtcNow;
    public virtual long Length => -1;
    public string Name => virtualFile.Name ?? String.Empty;
    public virtual string? PhysicalPath => null;

    public virtual Stream? CreateReadStream() => null;

    public static implicit operator T(FileInfoBase<T> fileInfo) => fileInfo.virtualFile;
}
