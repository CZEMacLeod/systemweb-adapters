// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.SystemWebAdapters;

public abstract class HttpHandlerBase
{
    protected async Task InvokeInner(HttpContext context, System.Web.IHttpHandler handler)
    {
        context.Items["CurrentHandler"] = handler;
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
        context.Items.Remove("CurrentHandler");
    }
}

public class HttpHandlerMiddleware<THandler> : HttpHandlerBase
    where THandler : System.Web.IHttpHandler, new()
{
    public HttpHandlerMiddleware(RequestDelegate next) { }

    public async Task Invoke(HttpContextCore coreContext)
    {
        var httpContext = (System.Web.HttpContext)coreContext;
        var handler = new THandler();
        await InvokeInner(httpContext, handler);
    }
}

public class HttpHandlerFactoryMiddleware<THandlerFactory> : HttpHandlerBase
    where THandlerFactory : System.Web.IHttpHandlerFactory, new()
{
    public HttpHandlerFactoryMiddleware(RequestDelegate next) { }

    public async Task Invoke(HttpContextCore coreContext)
    {
        var httpContext = (System.Web.HttpContext)coreContext;
        var factory = new THandlerFactory();

        var request = httpContext.Request;

        var handler = factory.GetHandler(httpContext,
            request.HttpMethod,
            request.RawUrl,
            request.PhysicalApplicationPath);

        if (handler is not null)
        {
            await InvokeInner(httpContext, handler);
            factory.ReleaseHandler(handler);
        }
        else
        {
            var response = httpContext.Response;
            response.Clear();
            response.SuppressContent = true;
        }
    }
}
