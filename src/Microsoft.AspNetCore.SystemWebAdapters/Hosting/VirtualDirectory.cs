// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.Extensions.FileProviders;

namespace System.Web.Hosting;

public abstract class VirtualDirectory : VirtualFileBase
{
    /// <summary>
    /// Contructs a VirtualDirectory, passing it the virtual path to the directory it represents
    /// </summary>
    /// <param name="virtualPath"></param>
    protected VirtualDirectory(string virtualPath) => this.virtualPath = VirtualPathUtility.AppendTrailingSlash(virtualPath);

    public override bool IsDirectory => true;

    /// <summary>
    /// Returns an object that enumerates all the children VirtualDirectory's of this directory.
    /// </summary>
    public abstract IEnumerable Directories { get; }

    /// <summary>
    /// Returns an object that enumerates all the children VirtualFile's of this directory.
    /// </summary>
    public abstract IEnumerable Files { get; }

    /// <summary>
    /// Returns an object that enumerates all the children VirtualDirectory's and VirtualFiles of this directory.
    /// </summary>
    public abstract IEnumerable Children { get; }

    //internal static implicit operator DirectoryContents(VirtualDirectory virtualDirectory) => new(virtualDirectory);

    internal class DirectoryContents : FileInfoBase<VirtualDirectory>, IDirectoryContents
    {
        internal DirectoryContents(VirtualDirectory directory) : base(directory) { }

        // Not sure if this would work or if we need to switch case and cast manually
        //public IEnumerable<IFileInfo> Children => virtualFile.Children.Cast<IFileInfo>();

        public IEnumerable<IFileInfo> Children => virtualFile.Children
            .Cast<VirtualFileBase>()
            .Select<VirtualFileBase, IFileInfo>(v => v switch
                {
                    VirtualFile file => new VirtualFile.FileInfo(file),
                    VirtualDirectory directory => new VirtualDirectory.DirectoryContents(directory),
                    _ => throw new HttpException($"Unknown child type. {v.GetType().Name} is not VirtualDirectory or VirtualFile")
                });

        public IEnumerator<IFileInfo> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    }
}
