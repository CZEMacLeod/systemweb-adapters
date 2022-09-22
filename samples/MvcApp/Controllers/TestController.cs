using System.Web;
using System.Web.Http;
using ClassLibrary;

namespace MvcApp.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [Route("request/info")]
        [HttpGet]
        public void GetData() => RequestInfo.WriteRequestInfo(false);

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

        [Route("httpruntime")]
        [HttpGet]
        public IHttpActionResult TestHttpRuntime()
        {
            return Json(new HttpRuntimePropertiesModel(), new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Converters = { new Newtonsoft.Json.Converters.VersionConverter() }
            });
        }


    }
}
