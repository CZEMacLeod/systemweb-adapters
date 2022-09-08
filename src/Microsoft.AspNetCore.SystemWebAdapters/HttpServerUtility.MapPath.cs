using Microsoft.AspNetCore.SystemWebAdapters.MapPath;
using Microsoft.Extensions.DependencyInjection;

namespace System.Web;

public partial class HttpServerUtility
{
    private IMapPath? _mapPath;
    public string MapPath(string? path)
    {
        _mapPath ??= _context.RequestServices.GetRequiredService<IMapPath>();
        return _mapPath.MapPath(path);
    }
}

