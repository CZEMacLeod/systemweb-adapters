// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

public class DefaultMapPath : AppPathMapPathBase
{
    public DefaultMapPath(HttpContext context) : base(context) { }

    protected override string AppPath => HttpRuntime.AppDomainAppPath;
}

