// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.SystemWebAdapters;

public enum RewriteScriptResourceType
{
    Directory,
    Either,
    File,
    Unspecified
}

public static class HttpHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseHttpHandlerMiddleWare<THandler>(this IApplicationBuilder builder)
        where THandler : class, System.Web.IHttpHandler, new()
    {
        return builder.UseMiddleware<HttpHandlerMiddleware<THandler>>();
    }

    public static IApplicationBuilder UseHttpHandlerFactoryMiddleWare<THandlerFactory>(this IApplicationBuilder builder)
    where THandlerFactory : class, System.Web.IHttpHandlerFactory, new()
    {
        return builder.UseMiddleware<HttpHandlerFactoryMiddleware<THandlerFactory>>();
    }

    private static IApplicationBuilder MapWhen(
        this IApplicationBuilder builder,
        string path,
        bool allowPathInfo,
        RewriteScriptResourceType resourceType,
        IFileProvider? fileProvider,
        Action<IApplicationBuilder> action
        )
    {
        var predicate = allowPathInfo
            ? path.StartsWith('*')
                ? (virtualPath => virtualPath.Contains(path[1..], StringComparison.OrdinalIgnoreCase))
                : (virtualPath => virtualPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            : (Func<string, bool>)(path.StartsWith('*')
                ? (virtualPath => virtualPath.EndsWith(path[1..], StringComparison.OrdinalIgnoreCase))
                : (virtualPath => virtualPath.Equals(path, StringComparison.OrdinalIgnoreCase)));

        return builder.MapWhen(ctx =>
        {
            var requestPath = ctx.Request.Path.HasValue ? ctx.Request.Path.Value[1..] : String.Empty;
            if (allowPathInfo)
            {
                if (predicate(requestPath))
                {
                    string? filePath;
                    string? pathInfo;
                    if (path.StartsWith('*'))
                    {
                        var index = requestPath.IndexOf(path[1..]);
                        filePath = "/" + requestPath[..(index)] + path[1..];
                        pathInfo = requestPath[(index + path.Length - 1)..];
                    }
                    else
                    {
                        filePath = "/" + path;
                        pathInfo = requestPath[path.Length..];
                    }
                    if (pathInfo != string.Empty && pathInfo[0] != '/') return false;
                    if (!CheckResourceType(ctx, resourceType, filePath, fileProvider)) return false;
                    ctx.Items["FilePath"] = filePath;
                    ctx.Items["PathInfo"] = pathInfo;
                    return true;
                }
            }
            else
            {
                if (predicate(requestPath))
                {
                    if (!CheckResourceType(ctx, resourceType, requestPath, fileProvider)) return false;
                    ctx.Items["FilePath"] = "/" + requestPath;
                    ctx.Items["PathInfo"] = string.Empty;
                    return true;
                }
            }
            return false;
        }, appBranch => action?.Invoke(appBranch));
    }

    public static IApplicationBuilder UseHttpHandler<THandler>(
        this IApplicationBuilder builder,
        string path,
        bool allowPathInfo = true,
        RewriteScriptResourceType resourceType = RewriteScriptResourceType.Unspecified,
        IFileProvider? fileProvider = null)
    where THandler : class, System.Web.IHttpHandler, new() =>
        builder.MapWhen(path, allowPathInfo, resourceType, fileProvider, when => when.UseHttpHandlerMiddleWare<THandler>());

    public static IApplicationBuilder UseHttpHandlerFactory<THandlerFactory>(
        this IApplicationBuilder builder,
        string path,
        bool allowPathInfo = true,
        RewriteScriptResourceType resourceType = RewriteScriptResourceType.Unspecified,
        IFileProvider? fileProvider = null)
    where THandlerFactory : class, System.Web.IHttpHandlerFactory, new() =>
        builder.MapWhen(path, allowPathInfo, resourceType, fileProvider, when => when.UseHttpHandlerFactoryMiddleWare<THandlerFactory>());

    private static bool CheckResourceType(this Http.HttpContext context, RewriteScriptResourceType resourceType, string path, IFileProvider? fileProvider)
    {
        if (resourceType != RewriteScriptResourceType.Unspecified)
        {
            fileProvider ??= context.RequestServices.GetRequiredService<IWebHostEnvironment>().WebRootFileProvider;
            var fi = fileProvider.GetFileInfo(path);
            if (!fi.Exists) return false;
            if (fi.IsDirectory && resourceType == RewriteScriptResourceType.File) return false;
            if (!fi.IsDirectory && resourceType == RewriteScriptResourceType.Directory) return false;
        }
        return true;
    }
}
