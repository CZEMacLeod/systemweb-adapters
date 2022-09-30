using System;
using System.Web.Caching;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.SystemWebAdapters
{
    internal class ChangeTokenCacheDependency : CacheDependency
    {
        private IChangeToken changeToken;
        private DateTimeOffset lastModified;

        public ChangeTokenCacheDependency(IChangeToken changeToken, DateTimeOffset lastModified)
        {
            this.changeToken = changeToken;
            this.lastModified = lastModified;
        }
    }
}