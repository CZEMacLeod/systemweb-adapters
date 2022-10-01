// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace System.Web.Caching;

public class CacheDependency : IDisposable
{
    private List<ChangeMonitor> changeMonitors = new();
    private bool hasChanged;
    private bool disposedValue;
    private DateTime utcLastModified;
    private Action<object, EventArgs>? dependencyChangedAction;
    private readonly string[]? filenames;

    internal CacheDependency()
    {
        FinishInit();
    }

    public CacheDependency(string filename) : this(new[] { filename }) { }

    public CacheDependency(string[] filenames)
    {
        this.filenames = filenames;
        changeMonitors.Add(new HostFileChangeMonitor(filenames.ToList()));
        FinishInit();
    }

    public CacheDependency(
        string[] filenames,
        string[] fullPathDependenciesArray,
        DateTime utcStart) : this(filenames)
    {
        this.filenames = filenames;
        if (filenames is not null && filenames.Length != 0)
        {
            changeMonitors.Add(new HostFileChangeMonitor(filenames.ToList()));
        }
        if (fullPathDependenciesArray is not null && fullPathDependenciesArray.Length != 0)
        {
            changeMonitors.Add(
                Hosting.HostingEnvironment.Cache.ObjectCache
                    .CreateCacheEntryChangeMonitor(fullPathDependenciesArray));
        }

        utcLastModified = utcStart;
    }

    protected internal void FinishInit()
    {
        foreach (var changeMonitor in changeMonitors)
        {
            changeMonitor?.NotifyOnChanged(ChangeMonitor_Changed);
        }
    }

    private void ChangeMonitor_Changed(object state)
    {

        hasChanged = true;
        dependencyChangedAction?.Invoke(this, EventArgs.Empty);
    }

    protected void SetUtcLastModified(DateTime utcLastModified) => this.utcLastModified = utcLastModified;

    public void SetCacheDependencyChanged(Action<object, EventArgs> dependencyChangedAction)
    {
        this.dependencyChangedAction = dependencyChangedAction;
    }

    public virtual string[] GetFileDependencies() => filenames ?? Array.Empty<string>();
    public bool HasChanged => changeMonitors.Any(cm => cm.HasChanged) || hasChanged;
    public DateTime UtcLastModified => changeMonitors
        .OfType<FileChangeMonitor>()
        .Select(fcm => fcm.LastModified.DateTime)
        .Concat(new[] { utcLastModified })
        .Max();

    internal IEnumerable<ChangeMonitor> ChangeMonitors { get => changeMonitors; }

    protected virtual void DependencyDispose()
    {

    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var changeMonitor in changeMonitors)
                {
                    changeMonitor?.Dispose();
                }
                changeMonitors.Clear();

                DependencyDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~CacheDependency()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    internal ChangeMonitor GetChangeMonitor() => new CacheDependencyChangeMonitor(this);

    internal class CacheDependencyChangeMonitor : ChangeMonitor
    {
        private readonly CacheDependency cacheDependency;
        private string? uniqueId;

        internal CacheDependencyChangeMonitor(CacheDependency cacheDependency)
        {
            this.cacheDependency = cacheDependency;
            cacheDependency.SetCacheDependencyChanged((state, _) => OnChanged(state));
            InitializationComplete();
        }

        public override string UniqueId => uniqueId ??= string.Join(":", cacheDependency.ChangeMonitors.Select(cm => cm.UniqueId));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cacheDependency?.Dispose();
            }
        }
    }
}
