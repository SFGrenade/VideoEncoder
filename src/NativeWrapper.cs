using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VideoEncoder;

public static class NativeWrapper
{
    [DllImport("VideoEncoderNative_Linux", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Linux_Init(string dir);
    private static bool Linux_Init_Wrap(string dir) => Native_Linux_Init(dir);
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_MacOS_Init(string dir);
    private static bool MacOS_Init_Wrap(string dir) => Native_MacOS_Init(dir);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "Init", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Windows_Init(string dir);
    private static bool Windows_Init_Wrap(string dir) => Native_Windows_Init(dir);
    internal static bool InitLibrary(string dir)
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.Linux:
                return Linux_Init_Wrap(dir);
            case OperatingSystemFamily.MacOSX:
                return MacOS_Init_Wrap(dir);
            case OperatingSystemFamily.Windows:
                return Windows_Init_Wrap(dir);
            default:
                return false;
        };
    }

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Linux_Deinit();
    private static bool Linux_Deinit_Wrap() => Native_Linux_Deinit();
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_MacOS_Deinit();
    private static bool MacOS_Deinit_Wrap() => Native_MacOS_Deinit();
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "Deinit", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
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

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Linux_StartNewSequence(int width, int height);
    private static bool Linux_StartNewSequence_Wrap(int width, int height) => Native_Linux_StartNewSequence(width, height); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_MacOS_StartNewSequence(int width, int height);
    private static bool MacOS_StartNewSequence_Wrap(int width, int height) => Native_MacOS_StartNewSequence(width, height);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "StartNewSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Windows_StartNewSequence(int width, int height);
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

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Linux_SendPngBytes(byte[] bytes, int size);
    private static bool Linux_SendPngBytes_Wrap(byte[] bytes) => Native_Linux_SendPngBytes(bytes, bytes.Length); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_MacOS_SendPngBytes(byte[] bytes, int size);
    private static bool MacOS_SendPngBytes_Wrap(byte[] bytes) => Native_MacOS_SendPngBytes(bytes, bytes.Length);
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "SendPngBytes", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Windows_SendPngBytes(byte[] bytes, int size);
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

    [DllImport("VideoEncoderNative_Linux", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_Linux_StopSequence();
    private static bool Linux_StopSequence_Wrap() => Native_Linux_StopSequence(); 
    [DllImport("VideoEncoderNative_MacOS", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
    private static extern bool Native_MacOS_StopSequence();
    private static bool MacOS_StopSequence_Wrap() => Native_MacOS_StopSequence();
    [DllImport("VideoEncoderNative_Windows", EntryPoint = "StopSequence", ExactSpelling = true, CharSet = CharSet.Ansi, PreserveSig = true)]
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