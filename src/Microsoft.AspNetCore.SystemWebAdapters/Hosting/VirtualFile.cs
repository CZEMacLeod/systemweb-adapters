// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace System.Web.Hosting;

public abstract class VirtualFile : VirtualFileBase
{
    /*
     * Contructs a VirtualFile, passing it the virtual path to the file it represents
     */
    protected VirtualFile(string virtualPath) => this.virtualPath = virtualPath;

    public override bool IsDirectory => false;
    
    /*
     * Returns a readonly stream to the file
     */
    public abstract Stream Open();

    #region "IFileInfo"

    //public static implicit operator FileInfo(VirtualFile virtualFile) => new(virtualFile);

    internal class FileInfo : Microsoft.AspNetCore.SystemWebAdapters.FileInfoBase<VirtualFile>
    {
        internal FileInfo(VirtualFile virtualFile) : base(virtualFile) { }

        public override long Length 
        {
            get
            {
                using var stream = CreateReadStream();
                return stream.Length;
            }
        }

        public override Stream CreateReadStream() => virtualFile.Open();

        //public static implicit operator VirtualFile(FileInfo fileInfo) => fileInfo.virtualFile;
    }
    #endregion
}
