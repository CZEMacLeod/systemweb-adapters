// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class CustomRootMapPath : AppPathMapPathBase
{
    private readonly IMapPathRoot _root;

    public CustomRootMapPath(IMapPathRoot root, IHttpContextAccessor context) : base(context) => _root = root;

    protected override string AppPath => _root.AppPath;
}
