// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.AspNetCore.SystemWebAdapters;

internal interface IHttpRuntime
{
    string AppDomainAppVirtualPath { get; }
    string? AppDomainAppPath { get; }

    string? AppDomainAppId { get; }
    string? AppDomainId { get; }
    Version? IISVersion { get; }
    Version TargetFramework { get; }

    string? InstanceID { get; }
    string? InstanceMetaPath { get; }
}
