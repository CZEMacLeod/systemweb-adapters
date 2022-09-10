// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SystemWebAdapters;

public abstract class HttpHandlerBase
{
    protected async Task InvokeInner(HttpContext context, System.Web.IHttpHandler handler)
    {
        if (handler is System.Web.IHttpAsyncHandler asyncHandler)
        {
            await Task.Factory.FromAsync(
                (callback, stateObject) => asyncHandler.BeginProcessRequest(context, callback, stateObject),
                asyncHandler.EndProcessRequest,
                null);
        }
        else
        {
            handler.ProcessRequest(context);
        }
    }
}

public class HttpHandlerMiddleware<THandler> : HttpHandlerBase
    where THandler :  System.Web.IHttpHandler, new()
{
    private readonly RequestDelegate _next;

    public HttpHandlerMiddleware(RequestDelegate next) => _next = next;

    // ASP.NET Core middleware that may terminate the request

    public async Task Invoke(HttpContextCore coreContext)
    {
        var httpContext = (System.Web.HttpContext)coreContext;
        var handler = new THandler();

        await InvokeInner(httpContext, handler);
    }
}

public class HttpHandlerFactoryMiddleware<THandlerFactory> : HttpHandlerBase
    where THandlerFactory :  System.Web.IHttpHandlerFactory, new()
{
    private readonly RequestDelegate _next;

    public HttpHandlerFactoryMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContextCore coreContext)
    {
        var httpContext = (System.Web.HttpContext)coreContext;
        var factory = new THandlerFactory();

        var handler = factory.GetHandler(httpContext,
            httpContext.Request.HttpMethod,
            httpContext.Request.RawUrl,
            httpContext.Request.PhysicalApplicationPath);

        await InvokeInner(httpContext, handler);

        factory.ReleaseHandler(handler);
    }
}
