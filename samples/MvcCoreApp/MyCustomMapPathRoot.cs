using Microsoft.AspNetCore.SystemWebAdapters.MapPath;

class MyCustomMapPathRoot : IMapPathRoot
{
    public string AppPath => "R:\\";
}
