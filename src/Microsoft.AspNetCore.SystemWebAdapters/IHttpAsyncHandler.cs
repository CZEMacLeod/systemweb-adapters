// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web;

public interface IHttpAsyncHandler : IHttpHandler
{
    public IAsyncResult BeginProcessRequest(System.Web.HttpContext context, AsyncCallback cb, object? extraData);
    public void EndProcessRequest(IAsyncResult result);
}
