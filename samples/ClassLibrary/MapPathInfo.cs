using System.IO;
using System.Web;
using Microsoft.AspNetCore.SystemWebAdapters.MapPath;

namespace ClassLibrary;

public class MapPathInfo
{
    private bool _hasWritten = false;

    public Stream WriteMapPathInfo(IMapPath mapPath)
    {
        var stream = new System.IO.MemoryStream();
        using (var writer = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8, 1024, true))
        {
            writer.WriteLine("{");
            Write(writer,"VirtualDirectory", HttpRuntime.AppDomainAppVirtualPath);
            Write(writer, "PhysicalDirectory", HttpRuntime.AppDomainAppPath);
            Write(writer, "RequestDirectory", mapPath.MapPath(null));
            Write(writer, "RequestDirectory2", mapPath.MapPath(""));
            Write(writer, "UploadedFiles", mapPath.MapPath("/UploadedFiles"));
            Write(writer, "RelativeFiles", mapPath.MapPath("UploadedFiles"));
            Write(writer, "AppFiles", mapPath.MapPath("~/UploadedFiles"));
            writer.WriteLine();
            writer.WriteLine("}");
            stream.Flush();
        }
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public void Write<T>(TextWriter writer, string name, T item)
    {
        if (_hasWritten)
        {
            writer.WriteLine(",");
        }

        _hasWritten = true;
        writer.Write("  ");
        writer.Write('\"');
        writer.Write(name);
        writer.Write("\" : \"");
        writer.Write(item);
        writer.Write('\"');
    }
}
