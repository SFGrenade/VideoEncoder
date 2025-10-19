using System.Reflection;
using Modding;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Modding.Utils;
using UnityEngine;
using VideoEncoder;
using UObject = UnityEngine.Object;
using MLogger = Modding.Logger;
using ULogger = UnityEngine.Debug;

namespace VideoEncoder;

[UsedImplicitly]
public class VideoEncoder : Mod
{
    public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    private static string _dir;

    // apparently a reference is needed for no crashes
    private static NativeWrapper.LogCallback _callbackDelegate;
    private static GameObject customCameraObj = null;

    public VideoEncoder() : base("Video Encoder")
    {
        _dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Screenshots");
        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }

        _callbackDelegate = DebugLog;
        NativeWrapper.InitLibrary(_dir, Application.persistentDataPath, Marshal.GetFunctionPointerForDelegate(_callbackDelegate));

        CreateCameraObject();
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        DebugLog("Initializing...");

        RegisterCallbacks();

        DebugLog("Initialized!");
    }

    private void RegisterCallbacks()
    {
        ModHooks.ApplicationQuitHook += () =>
        {
            // make sure the current recording stops
            customCameraObj.GetComponent<ScreenshotMb>().CleanupRenderTexture();
            NativeWrapper.DeinitLibrary();
        };
    }

    private void CreateCameraObject()
    {
        customCameraObj = new GameObject("TEST THING; DON'T REMOVE");
        UObject.DontDestroyOnLoad(customCameraObj);

        Camera screenshotCamera = customCameraObj.GetOrAddComponent<Camera>();
        screenshotCamera.CopyFrom(GameCameras.instance.mainCamera);

        screenshotCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        RenderTexture.active = screenshotCamera.targetTexture;

        ScreenshotMb screenshotter = customCameraObj.GetOrAddComponent<ScreenshotMb>();
        screenshotter.camera = screenshotCamera;
        screenshotter.dir = _dir;
    }

    internal static void DebugLog(string message)
    {
        string fmtMessage = "[VideoEncoder] - " + message;
        MLogger.LogDebug(fmtMessage);
        ULogger.Log(fmtMessage);
    }

    internal static void DebugLog(object message)
    {
        DebugLog($"{message}");
    }
}