using Microsoft.AspNetCore.SystemWebAdapters.MapPath;

namespace ClassLibrary;
class MyCustomMapPathRoot : IMapPathRoot
{
    public string AppPath => "R:\\";
}
