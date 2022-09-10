// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Based on https://docs.microsoft.com/en-us/previous-versions/aspnet/ms227433(v=vs.100)

using System;
using System.Web;
using System.Threading;

namespace HandlerLibrary;

public class HelloWorldAsyncHandler : IHttpAsyncHandler
{
    public bool IsReusable => false;

    public HelloWorldAsyncHandler()
    {
    }

    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    {
        context.Response.Write("<p>Begin IsThreadPoolThread is " + Thread.CurrentThread.IsThreadPoolThread + "</p>\r\n");
        var asynch = new Operation(cb, context, extraData);
        asynch.StartAsyncWork();
        return asynch;
    }

    public void EndProcessRequest(IAsyncResult result)
    {
    }

    public void ProcessRequest(HttpContext context) => throw new InvalidOperationException();


    private class Operation : IAsyncResult
    {
        private bool _completed;
        private object _state;
        private AsyncCallback _callback;
        private HttpContext _context;

        bool IAsyncResult.IsCompleted => _completed;
        WaitHandle IAsyncResult.AsyncWaitHandle => null;
        object IAsyncResult.AsyncState => _state;
        bool IAsyncResult.CompletedSynchronously => false;

        public Operation(AsyncCallback callback, HttpContext context, object state)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
        }

        public void StartAsyncWork() => ThreadPool.QueueUserWorkItem(new WaitCallback(StartAsyncTask), null);

        private void StartAsyncTask(object workItemState)
        {

            _context.Response.Write("<p>Completion IsThreadPoolThread is " + Thread.CurrentThread.IsThreadPoolThread + "</p>\r\n");

            _context.Response.Write("Hello World from Async Handler!");
            _completed = true;
            _callback(this);
        }
    }
}
