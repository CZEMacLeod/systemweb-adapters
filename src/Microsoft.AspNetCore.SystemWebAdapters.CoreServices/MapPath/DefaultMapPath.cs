// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters.MapPath;

internal class DefaultMapPath : AppPathMapPathBase
{
    private readonly IHttpRuntime _httpRuntime;

    public DefaultMapPath(IHttpRuntime httpRuntime, IHttpContextAccessor context) : base(context) => _httpRuntime = httpRuntime;

    protected override string AppPath => _httpRuntime.AppDomainAppPath;
}
