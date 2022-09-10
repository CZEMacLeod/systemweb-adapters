using System.Web;

namespace ClassLibrary
{
    public class InfoHttpHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            using (var writer = new SimpleJsonWriter(context.Response))
            {
                writer.Write("VirtualDirectory", HttpRuntime.AppDomainAppVirtualPath);
                writer.Write("PhysicalDirectory", HttpRuntime.AppDomainAppPath);
                writer.Write("RequestDirectory", context.Server.MapPath(null));
                writer.Write("RequestVirtualDirectory", context.Request.ApplicationPath);
                writer.Write("RawUrl", context.Request.RawUrl);
                writer.Write("Path", context.Request.Path);
                writer.Write("FilePath", context.Request.FilePath);
                writer.Write("PathInfo", context.Request.PathInfo);
                writer.Write("PhysicalApplicationPath", context.Request.PhysicalApplicationPath);
                writer.Write("PhysicalPath", context.Request.PhysicalPath);
                writer.Write("AppRelativeCurrentExecutionFilePath", context.Request.AppRelativeCurrentExecutionFilePath);
                writer.Write("UploadedFiles", context.Server.MapPath("/UploadedFiles"));
                writer.Write("RelativeFiles", context.Server.MapPath("UploadedFiles"));
                writer.Write("AppFiles", context.Server.MapPath("~/MyUploadedFiles"));
                context.Response.Output.Flush();
                context.Response.End();
            }
        }
    }
}
