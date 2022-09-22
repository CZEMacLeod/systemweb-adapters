// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.SystemWebAdapters;

internal static class NativeMethods
{
    private const int HR_OK = 0;
    private const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";
    private const string KERNEL32 = "kernel32.dll";

    [DllImport(KERNEL32, EntryPoint = "GetModuleHandleW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

    [DllImport(AspNetCoreModuleDll)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);

    [DllImport(AspNetCoreModuleDll, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern int http_get_server_variable(
        IntPtr pInProcessHandler,
        [MarshalAs(UnmanagedType.LPWStr)] string variableName,
        [MarshalAs(UnmanagedType.BStr)] out string value);

    internal static bool IsAspNetCoreModuleLoaded()
        => OperatingSystem.IsWindows() && GetModuleHandle(AspNetCoreModuleDll) != IntPtr.Zero;

    internal static IISConfigurationData HttpGetApplicationProperties()
    {
        var iisConfigurationData = new IISConfigurationData();
        Validate(http_get_application_properties(ref iisConfigurationData));
        return iisConfigurationData;
    }

    private static void Validate(int hr)
    {
        if (hr != HR_OK)
        {
            throw Marshal.GetExceptionForHR(hr)!;
        }
    }

    public static bool HttpTryGetServerVariable(IntPtr pInProcessHandler, string variableName, out string value)
    {
        return http_get_server_variable(pInProcessHandler, variableName, out value) == 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IISConfigurationData
    {
        public IntPtr pNativeApplication;
        [MarshalAs(UnmanagedType.BStr)]
        public string pwzFullApplicationPath;
        [MarshalAs(UnmanagedType.BStr)]
        public string pwzVirtualApplicationPath;
        public bool fWindowsAuthEnabled;
        public bool fBasicAuthEnabled;
        public bool fAnonymousAuthEnable;
        [MarshalAs(UnmanagedType.BStr)]
        public string pwzBindings;
        public uint maxRequestBodySize;
    }
}
