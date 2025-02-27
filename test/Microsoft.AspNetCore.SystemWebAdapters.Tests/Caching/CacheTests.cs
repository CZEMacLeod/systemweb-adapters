// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.SystemWebAdapters;
using Moq;
using Xunit;

namespace System.Web.Caching;

public class CacheTests
{
    private readonly Fixture _fixture;

    public CacheTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void GetEmpty()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);

        // Act
        var result = cache[_fixture.Create<string>()];

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Count()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>())
        {
            { _fixture.Create<string>(), new object(), DateTimeOffset.MaxValue },
            { _fixture.Create<string>(), new object(), DateTimeOffset.MaxValue },
            { _fixture.Create<string>(), new object(), DateTimeOffset.MaxValue },
        };

        var cache = new Cache(memCache);

        // Act
        var result = cache.Count;

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public void Enumerator()
    {
        // Arrange
        var key = _fixture.Create<string>();
        var value = new object();

        using var memCache = new MemoryCache(_fixture.Create<string>())
        {
            { key, value, DateTimeOffset.MaxValue },
        };

        var cache = new Cache(memCache);

        // Act
        var result = cache.GetEnumerator();

        // Assert
        var enumerator = (IDictionaryEnumerator)result;

        Assert.True(enumerator.MoveNext());
        Assert.Equal(key, enumerator.Key);
        Assert.Equal(value, enumerator.Value);

        Assert.False(result.MoveNext());
        result.Reset();

        Assert.True(result.MoveNext());
        Assert.Equal(key, enumerator.Key);
        Assert.Equal(value, enumerator.Value);
    }

    [Fact]
    public void InsertNoCallbacks()
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();
        var absoluteExpriration = _fixture.Create<DateTime>();
        var slidingExpiration = _fixture.Create<TimeSpan>();

        // Act
        cache.Insert(key, item, null, absoluteExpriration, slidingExpiration);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(absoluteExpriration) && e.SlidingExpiration.Equals(slidingExpiration)), null), Times.Once);
    }

    [Fact]
    public void InsertNoCallbacksConstants()
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();

        // Act
        cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(DateTimeOffset.MaxValue) && e.SlidingExpiration.Equals(Cache.NoSlidingExpiration)), null), Times.Once);
    }

    [InlineData(CacheItemPriority.Low, Runtime.Caching.CacheItemPriority.Default)]
    [InlineData(CacheItemPriority.BelowNormal, Runtime.Caching.CacheItemPriority.Default)]
    [InlineData(CacheItemPriority.Normal, Runtime.Caching.CacheItemPriority.Default)]
    [InlineData(CacheItemPriority.AboveNormal, Runtime.Caching.CacheItemPriority.Default)]
    [InlineData(CacheItemPriority.High, Runtime.Caching.CacheItemPriority.Default)]
    [InlineData(CacheItemPriority.NotRemovable, Runtime.Caching.CacheItemPriority.NotRemovable)]
    [Theory]
    public void InsertPriority(CacheItemPriority webPriority, Runtime.Caching.CacheItemPriority runtimePriority)
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();
        var absoluteExpriration = _fixture.Create<DateTime>();
        var slidingExpiration = _fixture.Create<TimeSpan>();

        // Act
        cache.Insert(key, item, null, absoluteExpriration, slidingExpiration, webPriority, onRemoveCallback: null);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(absoluteExpriration) && e.SlidingExpiration.Equals(slidingExpiration) && e.Priority == runtimePriority), null), Times.Once);
    }

    [Fact]
    public void InsertPriorityConstants()
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();

        // Act
        cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, onRemoveCallback: null);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(DateTimeOffset.MaxValue) && e.SlidingExpiration.Equals(Cache.NoSlidingExpiration) && e.Priority == Runtime.Caching.CacheItemPriority.Default), null), Times.Once);
    }

    [Fact]
    public void AddItemIndexer()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var key = _fixture.Create<string>();

        // Act
        cache[key] = item;

        // Assert
        Assert.Same(item, cache[key]);
    }

    [Fact]
    public void AddItemThenRemove()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var item2 = new object();
        var key = _fixture.Create<string>();
        Removal? removed = null;

        // Act
        var first = cache.Add(key, item, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, (key, item, reason) => removed = new(key, item, reason));
        cache.Remove(key);

        // Assert
        Assert.Null(first);
        Assert.NotNull(removed);
        Assert.Equal(key, removed!.Key);
        Assert.Equal(item, removed.Item);
        Assert.Equal(CacheItemRemovedReason.Removed, removed.Reason);
    }

    [Fact]
    public async Task UpdateItemCallback()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var item2 = new object();
        var key = _fixture.Create<string>();
        var updated = false;
        var slidingExpiration = TimeSpan.FromMilliseconds(1);
        CacheItemUpdateReason? updateReason = default;

        void Callback(string key, CacheItemUpdateReason reason, out object? expensiveObject, out CacheDependency? dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            expensiveObject = item2;
            dependency = null;
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = TimeSpan.FromMilliseconds(5);

            updated = true;
            updateReason = reason;
        }

        // Act
        cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, slidingExpiration, Callback);

        // Ensure sliding expiration has hit
        await Task.Delay(slidingExpiration);

        // Force cleanup to initiate callbacks on current thread
        memCache.Trim(100);

        // Assert
        Assert.True(updated);
        Assert.Same(cache[key], item2);
        Assert.Equal(CacheItemUpdateReason.Expired, updateReason);
    }

    [Fact]
    public async Task UpdateItemCallbackRemove()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var key = _fixture.Create<string>();
        var updated = false;
        var slidingExpiration = TimeSpan.FromMilliseconds(1);
        CacheItemUpdateReason? updateReason = default;

        void Callback(string key, CacheItemUpdateReason reason, out object? expensiveObject, out CacheDependency? dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            expensiveObject = null;
            dependency = null;
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = TimeSpan.FromMilliseconds(5);

            updated = true;
            updateReason = reason;
        }

        // Act
        cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, slidingExpiration, Callback);

        // Ensure sliding expiration has hit
        await Task.Delay(slidingExpiration);

        // Force cleanup to initiate callbacks on current thread
        memCache.Trim(100);

        // Assert
        Assert.True(updated);
        Assert.Null(cache[key]);
        Assert.Equal(CacheItemUpdateReason.Expired, updateReason);
    }

    [Fact]
    public void InsertItem()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var key = _fixture.Create<string>();

        // Act
        cache[key] = item;

        // Assert
        Assert.Same(item, cache[key]);
    }

    private sealed record Removal(string Key, object Item, CacheItemRemovedReason Reason);

    [Fact]
    public void InsertNoAbsoluteSlidingExpiration()
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();

        // Act
        cache.Insert(key, item);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(Cache.NoAbsoluteExpiration) && e.SlidingExpiration.Equals(Cache.NoSlidingExpiration)), null), Times.Once);
    }

    [Fact]
    public void InsertWithDependency()
    {
        // Arrange
        var memCache = new Mock<MemoryCache>(_fixture.Create<string>(), null);
        var cache = new Cache(memCache.Object);
        var key = _fixture.Create<string>();
        var item = new object();
        var cacheDependency = new Mock<CacheDependency>();

        // Act
        cache.Insert(key, item, cacheDependency.Object);

        // Assert
        memCache.Verify(m => m.Set(key, item, It.Is<CacheItemPolicy>(e => e.AbsoluteExpiration.Equals(Cache.NoAbsoluteExpiration) && e.SlidingExpiration.Equals(Cache.NoSlidingExpiration)), null), Times.Once);
    }

    [Fact]
    public async Task DependentFileCallback()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());
        var cache = new Cache(memCache);
        var item = new object();
        var key = _fixture.Create<string>();
        var updated = false;
        var slidingExpiration = TimeSpan.FromMilliseconds(1);
        CacheItemUpdateReason? updateReason = default;

        void Callback(string key, CacheItemUpdateReason reason, out object? expensiveObject, out CacheDependency? dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            expensiveObject = null;
            dependency = null;
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = TimeSpan.FromMilliseconds(5);

            updated = true;
            updateReason = reason;
        }

        var file = System.IO.Path.GetTempFileName();
        await System.IO.File.WriteAllTextAsync(file, key);

        using var cd = new CacheDependency(file);
        // Act
        cache.Insert(key, item, cd, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, Callback);

        // Ensure file is updated
        await System.IO.File.WriteAllTextAsync(file, DateTime.UtcNow.ToString("O"));

        // Small delay here to ensure that the file change notification happens (may fail tests if too fast)
        await Task.Delay(10);

        // Force cleanup to initiate callbacks on current thread
        memCache.Trim(100);

        // Assert
        Assert.True(updated);
        Assert.Null(cache[key]);
        Assert.Equal(CacheItemUpdateReason.DependencyChanged, updateReason);
    }

    [Fact]
    public async Task DependentItemCallback()
    {
        // Arrange
        using var memCache = new MemoryCache(_fixture.Create<string>());

        var cache = new Cache(memCache);
        var httpRuntime = new Mock<IHttpRuntime>();
        httpRuntime.Setup(s => s.Cache).Returns(cache);
        HttpRuntime.Current = httpRuntime.Object;

        var item1 = new object();
        var item2 = new object();
        var key1 = _fixture.Create<string>();
        var key2 = _fixture.Create<string>();
        var updateReason = new Dictionary<string, CacheItemUpdateReason>();
        var slidingExpiration = TimeSpan.FromMilliseconds(1);

        void Callback(string key, CacheItemUpdateReason reason, out object? expensiveObject, out CacheDependency? dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            expensiveObject = null;
            dependency = null;
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = Cache.NoSlidingExpiration;

            updateReason[key] = reason;
        }

        // Act
        cache.Insert(key1, item1, null, Cache.NoAbsoluteExpiration, slidingExpiration, Callback);

        using var cd = new CacheDependency(null, new[] { key1 });
        cache.Insert(key2, item2, cd, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, Callback);

        Assert.Empty(updateReason);

        // Ensure sliding expiration has hit
        await Task.Delay(slidingExpiration);

        // Force cleanup to initiate callbacks on current thread
        memCache.Trim(100);

        // Assert
        Assert.Contains(key1, updateReason.Keys);
        Assert.Contains(key2, updateReason.Keys);

        Assert.Null(cache[key1]);
        Assert.Null(cache[key2]);

        Assert.Equal(CacheItemUpdateReason.Expired, updateReason[key1]);
        Assert.Equal(CacheItemUpdateReason.DependencyChanged, updateReason[key2]);
    }
}
