using System.Web;
using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.MapPath;

namespace MvcCoreApp.Controllers
{
    [PreBufferRequestStream]
    [BufferResponseStream]
    public class TestController : Controller
    {
        [HttpGet]
        [Session(IsReadOnly = true)]
        [Route("/api/test/request/info")]
        public void Get([FromQuery] bool? suppress = false) => RequestInfo.WriteRequestInfo(suppress ?? false);

        [Route("/api/test/request/cookie")]
        public void TestRequestCookie() => CookieTests.RequestCookies(HttpContext);

        [Route("/api/test/response/cookie")]
        [HttpGet]
        public void TestResponseCookie(bool shareable = false)
        {
            // Force public cache control for testing Shareable behavior
            HttpContext.Response.Headers["Cache-Control"] = "public";

            CookieTests.ResponseCookies(HttpContext, shareable);
        }

        [Route("/api/test/server/mappath")]
        [HttpGet]
        public IActionResult GetMapPath([FromServices] IMapPath mapPath)
        {
            var mpi = new MapPathInfo();
            var stream = mpi.WriteMapPathInfo(mapPath);
            return File(stream, "application/json");
        }
    }
}
