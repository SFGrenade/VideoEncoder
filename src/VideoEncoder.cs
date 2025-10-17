using System.Reflection;
using Modding;
using System.Collections.Generic;
using System.IO;
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

    public VideoEncoder() : base("Video Encoder")
    {
        _dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Screenshots");
        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }
        NativeWrapper.InitLibrary(_dir, Screen.width, Screen.height);
    }
    
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        DebugLog("Initializing...");

        ModHooks.ApplicationQuitHook += () => {
            NativeWrapper.DeinitLibrary();
        };
        RegisterCallbacks();

        DebugLog("Initialized!");
    }

    private void RegisterCallbacks()
    {
        On.GameCameras.Awake += On_GameCameras_Awake;
        if (GameCameras.instance != null)
            On_GameCameras_Awake(_ => { }, GameCameras.instance);
    }

    private void On_GameCameras_Awake(On.GameCameras.orig_Awake orig, GameCameras self)
    {
        orig(self);
        ScreenshotMb screenshotter = self.mainCamera.gameObject.GetOrAddComponent<ScreenshotMb>();
        screenshotter.camera = self.mainCamera;
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