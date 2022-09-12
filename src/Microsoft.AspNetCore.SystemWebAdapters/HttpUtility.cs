using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace System.Web;

public class HttpUtility
{
    [return: NotNullIfNotNull("value")]
    public static string? HtmlEncode(object? value)
    {
        if (value is null) return null;
        if (value is IHtmlString ihtmlString) return ihtmlString.ToHtmlString();
        if (value is Microsoft.AspNetCore.Html.HtmlString htmlString) return htmlString.ToString();
        if (value is Microsoft.AspNetCore.Html.IHtmlContent htmlContent)
        {
            var writer = new StringWriter();
            htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            var result = writer.ToString();
            return result;
        }
        return HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty);
    }
}
