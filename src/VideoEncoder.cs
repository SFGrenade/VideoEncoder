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

    private Camera screenshotCamera = null;
    private ScreenshotMb screenshotter = null;
    private RenderTexture activeRenderTexture = null;

    public VideoEncoder() : base("Video Encoder")
    {
        _dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Screenshots");
        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }

        _callbackDelegate = DebugLog;
        NativeWrapper.InitLibrary(_dir, Application.persistentDataPath, Marshal.GetFunctionPointerForDelegate(_callbackDelegate));
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        DebugLog("Initializing...");

        ModHooks.ApplicationQuitHook += () => { NativeWrapper.DeinitLibrary(); };
        RegisterCallbacks();

        DebugLog("Initialized!");
    }

    private void RegisterCallbacks()
    {
        On.GameCameras.Awake += On_GameCameras_Awake;
        On.GameCameras.OnDestroy += On_GameCameras_OnDestroy;
        if (GameCameras.instance != null)
        {
            On_GameCameras_Awake(_ => { }, GameCameras.instance);
        }
    }

    private void On_GameCameras_Awake(On.GameCameras.orig_Awake orig, GameCameras self)
    {
        orig(self);

        GameObject customCameraObj = new GameObject("TEST THING; DON'T REMOVE");
        UObject.DontDestroyOnLoad(customCameraObj);
        customCameraObj.transform.position = self.mainCamera.transform.position;
        customCameraObj.transform.SetParent(self.mainCamera.transform, true);

        screenshotCamera = customCameraObj.GetOrAddComponent<Camera>();
        screenshotCamera.clearFlags = CameraClearFlags.Skybox;
        screenshotCamera.backgroundColor = new Color(0, 0, 0, 1.0f);
        screenshotCamera.gateFit = Camera.GateFitMode.Horizontal;
        screenshotCamera.sensorSize = new Vector2(36, 24);
        screenshotCamera.lensShift = new Vector2(0, 0);
        screenshotCamera.focalLength = 50;
        screenshotCamera.nearClipPlane = 10;
        screenshotCamera.farClipPlane = 1000;
        screenshotCamera.fieldOfView = 24;
        screenshotCamera.orthographic = false;
        screenshotCamera.orthographicSize = 480;
        screenshotCamera.depth = 50;
        screenshotCamera.cullingMask = -1073741857;
        screenshotCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        screenshotCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        screenshotCamera.allowHDR = false;
        screenshotCamera.allowMSAA = true;
        screenshotCamera.allowDynamicResolution = false;
        screenshotCamera.forceIntoRenderTexture = false;
        screenshotCamera.useOcclusionCulling = true;
        screenshotCamera.stereoConvergence = 10;
        screenshotCamera.stereoSeparation = 0.022f;
        screenshotCamera.transparencySortMode = TransparencySortMode.Orthographic;

        activeRenderTexture = RenderTexture.active;
        RenderTexture.active = screenshotCamera.targetTexture;

        screenshotter = customCameraObj.GetOrAddComponent<ScreenshotMb>();
        screenshotter.camera = screenshotCamera;
        screenshotter.dir = _dir;
    }

    private void On_GameCameras_OnDestroy(On.GameCameras.orig_OnDestroy orig, GameCameras self)
    {
        orig(self);
        UObject.Destroy(screenshotCamera.gameObject);
        // screenshotCamera.targetTexture = null;
        // RenderTexture.active = activeRenderTexture;
        // UObject.DestroyImmediate(screenshotter);
        // UObject.DestroyImmediate(screenshotCamera);
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