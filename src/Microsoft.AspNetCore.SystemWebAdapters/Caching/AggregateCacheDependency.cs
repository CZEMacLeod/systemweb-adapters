using System.Collections.Generic;
using System.Text;

namespace System.Web.Caching;

public class AggregateCacheDependency : CacheDependency
{
    private List<CacheDependency>? _dependencies;

    public AggregateCacheDependency() : base() { }

    public void Add(params System.Web.Caching.CacheDependency[] dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);
        if (disposedValue)
        {
            throw new ObjectDisposedException(nameof(AggregateCacheDependency));
        }

        dependencies = (CacheDependency[])dependencies.Clone();

        foreach (var dependency in dependencies)
        {
            if (dependency is null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (!dependency.TakeOwnership())
            {
                throw new InvalidOperationException("CacheDependency used more than once");
            }
        }

        var utcLastModified = DateTime.MinValue;
        var hasChanged = false;
        lock (this)
        {
            _dependencies ??= new List<CacheDependency>();

            _dependencies.AddRange(dependencies);

            foreach (var dependency in dependencies)
            {
                dependency.SetCacheDependencyChanged((object sender, EventArgs args) =>
                {
                    NotifyDependencyChanged(sender, args);
                });

                if (dependency.UtcLastModified > utcLastModified)
                {
                    utcLastModified = dependency.UtcLastModified;
                }

                if (dependency.HasChanged)
                {
                    hasChanged = true;
                    break;
                }
            }
        }

        uniqueIdInitialized = false;    // Clear the flag as the name will change
        uniqueId = null;

        SetUtcLastModified(utcLastModified);

        // if a dependency has changed, notify others that we have changed.
        if (hasChanged)
        {
            NotifyDependencyChanged(this, EventArgs.Empty);
        }
    }
    public override string[]? GetFileDependencies()
    {
        if (_dependencies is null)
        {
            return null;
        }

        List<string>? fileNames = null;
        foreach (var dependency in _dependencies)
        {
            if (dependency.GetType() == typeof(CacheDependency) ||
               dependency.GetType() == typeof(AggregateCacheDependency))
            {
                var tmpFileNames = dependency.GetFileDependencies();
                if (tmpFileNames != null)
                {
                    fileNames ??= new();
                    fileNames.AddRange(tmpFileNames);
                }
            }
        }
        return fileNames?.ToArray();
    }

    protected override void DependencyDispose()
    {
        if (_dependencies != null)
        {
            foreach (var d in _dependencies)
            {
                d.Dispose();
            }
            _dependencies = null;
        }
        base.DependencyDispose();
    }


    public override string? GetUniqueID()
    {
        if (_dependencies is null || _dependencies.Count==0)
        {
            return null;
        }
        if (!uniqueIdInitialized)
        {
            StringBuilder? sb = null;
            foreach (var dependency in _dependencies)
            {
                var id = dependency.GetUniqueID();
                if (id is null)
                {
                    uniqueIdInitialized = true;
                    return null;
                }
                sb ??= new StringBuilder();
                sb.Append(id);
            }
            uniqueId = sb?.ToString();
            uniqueIdInitialized = true;
        }

        return uniqueId;
    }
}
