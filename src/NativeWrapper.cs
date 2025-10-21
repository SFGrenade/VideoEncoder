using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VideoEncoder;

public static class NativeWrapper
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogCallback([MarshalAs(UnmanagedType.LPStr)] string message);

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_Init([MarshalAs(UnmanagedType.LPStr)] string mod_dir, [MarshalAs(UnmanagedType.LPStr)] string save_dir, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    private static bool Linux_Init_Wrap(string mod_dir, string save_dir, IntPtr callback) => Native_Linux_Init(mod_dir, save_dir, callback);
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_Init([MarshalAs(UnmanagedType.LPStr)] string mod_dir, [MarshalAs(UnmanagedType.LPStr)] string save_dir, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    private static bool MacOS_Init_Wrap(string mod_dir, string save_dir, IntPtr callback) => Native_MacOS_Init(mod_dir, save_dir, callback);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_Init([MarshalAs(UnmanagedType.LPStr)] string mod_dir, [MarshalAs(UnmanagedType.LPStr)] string save_dir, [MarshalAs(UnmanagedType.FunctionPtr)] IntPtr callback);
    private static bool Windows_Init_Wrap(string mod_dir, string save_dir, IntPtr callback) => Native_Windows_Init(mod_dir, save_dir, callback);
    internal static bool InitLibrary(string mod_dir, string save_dir, IntPtr callback)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_Init_Wrap(mod_dir, save_dir, callback);
            case OperatingSystemFamily.MacOSX:
                return MacOS_Init_Wrap(mod_dir, save_dir, callback);
            case OperatingSystemFamily.Windows:
                return Windows_Init_Wrap(mod_dir, save_dir, callback);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SetFileExtension", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_SetFileExtension([MarshalAs(UnmanagedType.LPStr)] string file_extension);
    private static bool Linux_SetFileExtension_Wrap(string file_extension) => Native_Linux_SetFileExtension(file_extension);
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SetFileExtension", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_SetFileExtension([MarshalAs(UnmanagedType.LPStr)] string file_extension);
    private static bool MacOS_SetFileExtension_Wrap(string file_extension) => Native_MacOS_SetFileExtension(file_extension);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SetFileExtension", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_SetFileExtension([MarshalAs(UnmanagedType.LPStr)] string file_extension);
    private static bool Windows_SetFileExtension_Wrap(string file_extension) => Native_Windows_SetFileExtension(file_extension);
    internal static bool SetFileExtension(string file_extension)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_SetFileExtension_Wrap(file_extension);
            case OperatingSystemFamily.MacOSX:
                return MacOS_SetFileExtension_Wrap(file_extension);
            case OperatingSystemFamily.Windows:
                return Windows_SetFileExtension_Wrap(file_extension);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SetCodecOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool Linux_SetCodecOption_Wrap(string key, string value) => Native_Linux_SetCodecOption(key, value);
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SetCodecOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool MacOS_SetCodecOption_Wrap(string key, string value) => Native_MacOS_SetCodecOption(key, value);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SetCodecOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool Windows_SetCodecOption_Wrap(string key, string value) => Native_Windows_SetCodecOption(key, value);
    internal static bool SetCodecOption(string key, string value)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_SetCodecOption_Wrap(key, value);
            case OperatingSystemFamily.MacOSX:
                return MacOS_SetCodecOption_Wrap(key, value);
            case OperatingSystemFamily.Windows:
                return Windows_SetCodecOption_Wrap(key, value);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SetMediaOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_SetMediaOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool Linux_SetMediaOption_Wrap(string key, string value) => Native_Linux_SetMediaOption(key, value);
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SetMediaOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_SetMediaOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool MacOS_SetMediaOption_Wrap(string key, string value) => Native_MacOS_SetMediaOption(key, value);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SetMediaOption", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_SetMediaOption([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value);
    private static bool Windows_SetMediaOption_Wrap(string key, string value) => Native_Windows_SetMediaOption(key, value);
    internal static bool SetMediaOption(string key, string value)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_SetMediaOption_Wrap(key, value);
            case OperatingSystemFamily.MacOSX:
                return MacOS_SetMediaOption_Wrap(key, value);
            case OperatingSystemFamily.Windows:
                return Windows_SetMediaOption_Wrap(key, value);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_Deinit();
    private static bool Linux_Deinit_Wrap() => Native_Linux_Deinit();
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_Deinit();
    private static bool MacOS_Deinit_Wrap() => Native_MacOS_Deinit();
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_Deinit();
    private static bool Windows_Deinit_Wrap() => Native_Windows_Deinit();
    internal static bool DeinitLibrary()
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_Deinit_Wrap();
            case OperatingSystemFamily.MacOSX:
                return MacOS_Deinit_Wrap();
            case OperatingSystemFamily.Windows:
                return Windows_Deinit_Wrap();
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_StartNewSequence([MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height);
    private static bool Linux_StartNewSequence_Wrap(int width, int height) => Native_Linux_StartNewSequence(width, height); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_StartNewSequence([MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height);
    private static bool MacOS_StartNewSequence_Wrap(int width, int height) => Native_MacOS_StartNewSequence(width, height);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_StartNewSequence([MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height);
    private static bool Windows_StartNewSequence_Wrap(int width, int height) => Native_Windows_StartNewSequence(width, height);
    internal static bool StartNewSequence(int width, int height)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_StartNewSequence_Wrap(width, height);
            case OperatingSystemFamily.MacOSX:
                return MacOS_StartNewSequence_Wrap(width, height);
            case OperatingSystemFamily.Windows:
                return Windows_StartNewSequence_Wrap(width, height);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_SendPngBytes([MarshalAs(UnmanagedType.LPArray)] byte[] bytes, [MarshalAs(UnmanagedType.I4)] int size);
    private static bool Linux_SendPngBytes_Wrap(byte[] bytes) => Native_Linux_SendPngBytes(bytes, bytes.Length); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_SendPngBytes([MarshalAs(UnmanagedType.LPArray)] byte[] bytes, [MarshalAs(UnmanagedType.I4)] int size);
    private static bool MacOS_SendPngBytes_Wrap(byte[] bytes) => Native_MacOS_SendPngBytes(bytes, bytes.Length);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_SendPngBytes([MarshalAs(UnmanagedType.LPArray)] byte[] bytes, [MarshalAs(UnmanagedType.I4)] int size);
    private static bool Windows_SendPngBytes_Wrap(byte[] bytes) => Native_Windows_SendPngBytes(bytes, bytes.Length);
    internal static bool SendPngBytes(byte[] bytes)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_SendPngBytes_Wrap(bytes);
            case OperatingSystemFamily.MacOSX:
                return MacOS_SendPngBytes_Wrap(bytes);
            case OperatingSystemFamily.Windows:
                return Windows_SendPngBytes_Wrap(bytes);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SendRawBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_SendRawBytes([MarshalAs(UnmanagedType.LPArray)] IntPtr bytes, [MarshalAs(UnmanagedType.I4)] int size, [MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height, [MarshalAs(UnmanagedType.R8)] double timestamp);
    private static bool Linux_SendRawBytes_Wrap(IntPtr bytes, int length, int width, int height, double timestamp) => Native_Linux_SendRawBytes(bytes, length, width, height, timestamp); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SendRawBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_SendRawBytes([MarshalAs(UnmanagedType.LPArray)] IntPtr bytes, [MarshalAs(UnmanagedType.I4)] int size, [MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height, [MarshalAs(UnmanagedType.R8)] double timestamp);
    private static bool MacOS_SendRawBytes_Wrap(IntPtr bytes, int length, int width, int height, double timestamp) => Native_MacOS_SendRawBytes(bytes, length, width, height, timestamp);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SendRawBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_SendRawBytes([MarshalAs(UnmanagedType.LPArray)] IntPtr bytes, [MarshalAs(UnmanagedType.I4)] int size, [MarshalAs(UnmanagedType.I4)] int width, [MarshalAs(UnmanagedType.I4)] int height, [MarshalAs(UnmanagedType.R8)] double timestamp);
    private static bool Windows_SendRawBytes_Wrap(IntPtr bytes, int length, int width, int height, double timestamp) => Native_Windows_SendRawBytes(bytes, length, width, height, timestamp);
    internal static bool SendRawBytes(IntPtr bytes, int length, int width, int height, double timestamp)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_SendRawBytes_Wrap(bytes, length, width, height, timestamp);
            case OperatingSystemFamily.MacOSX:
                return MacOS_SendRawBytes_Wrap(bytes, length, width, height, timestamp);
            case OperatingSystemFamily.Windows:
                return Windows_SendRawBytes_Wrap(bytes, length, width, height, timestamp);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Linux_StopSequence();
    private static bool Linux_StopSequence_Wrap() => Native_Linux_StopSequence(); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_MacOS_StopSequence();
    private static bool MacOS_StopSequence_Wrap() => Native_MacOS_StopSequence();
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool Native_Windows_StopSequence();
    private static bool Windows_StopSequence_Wrap() => Native_Windows_StopSequence();
    internal static bool StopSequence()
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_StopSequence_Wrap();
            case OperatingSystemFamily.MacOSX:
                return MacOS_StopSequence_Wrap();
            case OperatingSystemFamily.Windows:
                return Windows_StopSequence_Wrap();
            default:
                return false;
        };
    }
}