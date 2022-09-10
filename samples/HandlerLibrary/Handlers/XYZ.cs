// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://docs.microsoft.com/en-us/dotnet/api/system.web.ihttphandlerfactory.gethandler?view=netframework-4.8#system-web-ihttphandlerfactory-gethandler(system-web-httpcontext-system-string-system-string-system-string)

using System.Web;

namespace HandlerLibrary.Handlers;

// Class definition for xyz.aspx handler.
public class XYZ : IHttpHandler
{
    public virtual void ProcessRequest(HttpContext context)
    {
        context.Response.Write("<html><body>");
        context.Response.Write("<p>XYZ Handler</p>\n");
        context.Response.Write("</body></html>");
    }

    public virtual bool IsReusable
    {
        get { return true; }
    }
}

