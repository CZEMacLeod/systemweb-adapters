using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.Hosting;
using VPPLibrary;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDirectoryBrowser();
builder.Services.AddSystemWebAdapters()
    .WrapAspNetCoreSession()
    .AddSessionSerializer()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<int>("callCount");
    })
    .AddVirtualPathProvider<SamplePathProvider>()
    .AddVirtualPathProvidersAsStaticFileProvider(options =>
    {
        var mimeTypes = new FileExtensionContentTypeProvider();
        mimeTypes.Mappings[".vrf"] = "text/html";
        options.ContentTypeProvider = mimeTypes;
        //options.ServeUnknownFileTypes = true;
        // We need to either ServeUnknownFileTypes or use a custom FileExtensionContentTypeProvider as .vrf files don't have a filetype set
        // This is handled in asp.net 4.x in web.config with the staticContext/mimeMap entry
    });

builder.Services.AddDistributedMemoryCache();

//builder.Services.AddOptions<DefaultFilesOptions>().PostConfigure(options => options.DefaultFileNames = new[] { "index.html" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseDirectoryBrowser();

app.UseRouting();

app.UseSession();

app.UseSystemWebAdapters();

app.MapGet("/session", (HttpContext context) =>
{
    var adapter = (System.Web.HttpContext)context;

    if (adapter.Session!["callCount"] is not int count)
    {
        count = 0;
    }

    adapter.Session!["callCount"] = ++count;

    return $"This endpoint has been hit {count} time(s) this session";
}).RequireSystemWebAdapterSession();

app.Run();
