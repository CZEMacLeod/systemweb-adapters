using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.Hosting;
using VPPLibrary;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSystemWebAdapters()
    .WrapAspNetCoreSession()
    .AddSessionSerializer()
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<int>("callCount");
    })
    .AddVirtualPathProvidersAsStaticFileProvider();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IVirtualPathProvider, SamplePathProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

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
