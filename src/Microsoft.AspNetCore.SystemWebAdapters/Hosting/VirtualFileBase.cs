// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using Microsoft.Extensions.FileProviders;

namespace System.Web.Hosting;

public abstract class VirtualFileBase
{
    internal string? virtualPath;
    private string? name;

    public virtual string? Name => virtualPath is null ? null : name ??= VirtualPathUtility.GetFileName(virtualPath);

    public string VirtualPath => virtualPath ?? String.Empty;

    public abstract bool IsDirectory { get; }
}
