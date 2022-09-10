// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://docs.microsoft.com/en-us/dotnet/api/system.web.ihttphandlerfactory.gethandler?view=netframework-4.8#system-web-ihttphandlerfactory-gethandler(system-web-httpcontext-system-string-system-string-system-string)

using System;
using System.Web;

namespace HandlerLibrary;
// Factory class that creates a handler object based on a request
// for either abc.aspx or xyz.aspx as specified in the Web.config file.
public class MyHandlerFactory : IHttpHandlerFactory
{
    //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public virtual IHttpHandler GetHandler(HttpContext context,
                                           string requestType,
                                           string url,
                                           string pathTranslated)
    {
        string fname = url.Substring(url.LastIndexOf('/') + 1);
        string cname = fname.Substring(0, fname.IndexOf('.'));
        string className = "HandlerFactoryTest.Handlers." + cname.ToUpperInvariant();

        object h = null;

        // Try to create the handler object.
        try
        {
            // Create the handler by calling class abc or class xyz.
            h = Activator.CreateInstance(Type.GetType(className));
        }
        catch (Exception e)
        {
            throw new HttpException("Factory couldn't create instance " +
                                    "of type " + className, e);
        }
        return (IHttpHandler)h;
    }

    // This is a must override method.
    public virtual void ReleaseHandler(IHttpHandler handler)
    {
    }
}
