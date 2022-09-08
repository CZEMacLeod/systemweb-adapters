using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ClassLibrary;
using Microsoft.AspNetCore.SystemWebAdapters.MapPath;

namespace MvcApp.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [Route("request/info")]
        [HttpGet]
        public void GetData() => RequestInfo.WriteRequestInfo(false);

        [Route("server/mappath")]
        [HttpGet]
        public HttpResponseMessage GetMapPath(HttpRequestMessage request)
        {
            var mp = new NativeMapPath(HttpContext.Current);
            var mpi = new MapPathInfo();
            var stream = mpi.WriteMapPathInfo(mp);
            var response = request.CreateResponse();
            response.Content = new StreamContent(stream)
            {
                Headers =
                {
                    ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                }
            };
            return response;
        }


        [Route("request/cookie")]
        [HttpGet]
        public void TestRequestCookie() => CookieTests.RequestCookies(HttpContext.Current);

        [Route("response/cookie")]
        [HttpGet]
        public void TestResponseCookie(bool shareable = false)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
            CookieTests.ResponseCookies(HttpContext.Current, shareable);
        }
    }
}
