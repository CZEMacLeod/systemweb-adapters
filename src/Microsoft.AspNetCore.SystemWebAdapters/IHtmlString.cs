// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace System.Web;

/// <summary>
/// Represents an HTML-encoded string that should not be encoded again.
/// </summary>
public interface IHtmlString : IHtmlContent
{
    string ToHtmlString();
    void IHtmlContent.WriteTo(TextWriter writer, HtmlEncoder encoder) => writer.Write(ToHtmlString());
}
