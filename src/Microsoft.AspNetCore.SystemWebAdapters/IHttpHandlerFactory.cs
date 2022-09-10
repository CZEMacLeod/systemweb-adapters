// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web;

public interface IHttpHandlerFactory
{
    IHttpHandler GetHandler(System.Web.HttpContext context, string requestType, string? url, string pathTranslated);
    void ReleaseHandler(System.Web.IHttpHandler handler);
}
