using System.Web;
using VPPLibrary;

namespace VPPApp;

public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        SamplePathProvider sampleProvider = new SamplePathProvider();
        System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(sampleProvider);
    }
}
