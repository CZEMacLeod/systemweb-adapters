// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://docs.microsoft.com/en-us/previous-versions/aspnet/ms228090(v=vs.100)

using System.Web;

namespace HandlerLibrary;

public class HelloWorldHandler : IHttpHandler
{
    public HelloWorldHandler()
    {
    }

    public void ProcessRequest(HttpContext context)
    {
        var Response = context.Response;
        // This handler is called whenever a file ending 
        // in .sample is requested. A file with that extension
        // does not need to exist.
        Response.Write("<html>");
        Response.Write("<body>");
        Response.Write("<h1>Hello from a synchronous custom HTTP handler.</h1>");
        Response.Write("</body>");
        Response.Write("</html>");
    }

    // To enable pooling, return true here.
    // This keeps the handler in memory.
    public bool IsReusable
        => false;
}
