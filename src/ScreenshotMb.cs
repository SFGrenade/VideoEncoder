using System;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace VideoEncoder;

public class ScreenshotMb : MonoBehaviour
{
    public static ScreenshotMb Instance = null;

    public string dir;
    public Camera camera;

    private bool _setupDone = false;
    private bool _shouldTakeScreenshots = false;
    private bool _doTakeScreenshots = false;
    private int _screenShotTimingNow = 0;
    private double _startOfSequence = 0.0;

    public bool IsRecording() => _doTakeScreenshots;

    [UsedImplicitly]
    private void Start()
    {
        Instance = this;
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        if (VideoEncoder.GlobalSettings.CapRecordingToFixedUpdate)
        {
            _screenShotTimingNow++;
        }
    }

    [UsedImplicitly]
    private void Update()
    {
        if (!_setupDone && GameCameras.instance != null && GameCameras.instance.mainCamera != null)
        {
            _setupDone = true;
            RenderTexture prevTarget = GetComponent<Camera>().targetTexture;
            GetComponent<Camera>().CopyFrom(GameCameras.instance.mainCamera);
            GetComponent<Camera>().targetTexture = prevTarget;
            if (VideoEncoder.GlobalSettings.StartRecordingOnStart)
            {
                // toggle recording once setup is done, which is practically at the start and activating the recording
                ToggleRecording();
            }
        }

        if (!VideoEncoder.GlobalSettings.CapRecordingToFixedUpdate)
        {
            _screenShotTimingNow++;
        }

        if (HeroController.instance != null)
        {
            transform.position = new Vector3(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y, -38.1f);
        }
        else if (GameCameras.instance != null && GameCameras.instance.mainCamera != null)
        {
            transform.position = new Vector3(GameCameras.instance.mainCamera.transform.position.x, GameCameras.instance.mainCamera.transform.position.y, -38.1f);
        }

        if (Input.GetKeyDown(VideoEncoder.GlobalSettings.StartStopKey))
        {
            ToggleRecording();
        }
    }

    public void ToggleRecording()
    {
        _shouldTakeScreenshots = !_shouldTakeScreenshots;
    }

    const int MaxInFlight = 3;
    int inFlight = 0;

    [UsedImplicitly]
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        Graphics.Blit(src, dest);

        if (!_setupDone)
            return;

        // this is true when _doTakeScreenshots is true and then _screenShotTimingNow is the same as the fps denumerator
        if (_doTakeScreenshots && (_screenShotTimingNow >= VideoEncoder.GlobalSettings.RenderFpsDenumerator))
        {
            if (inFlight < MaxInFlight)
            {
                double now = Time.realtimeSinceStartup;
                AsyncGPUReadback.Request(src, 0, TextureFormat.RGB24, request =>
                {
                    inFlight--;
                    if (request.hasError || !_doTakeScreenshots)
                        return;

                    NativeArray<byte> data = request.GetData<byte>();
                    unsafe
                    {
                        NativeWrapper.SendRawBytes((IntPtr)data.GetUnsafePtr(), data.Length, src.width, src.height, now - _startOfSequence);
                    }
                });
                inFlight++;
                _screenShotTimingNow = 0;
            }
        }

        if (_doTakeScreenshots && !_shouldTakeScreenshots)
        {
            CleanupRenderTexture();
        }
        else if (!_doTakeScreenshots && _shouldTakeScreenshots)
        {
            PrepareRenderTexture();
        }
    }

    internal void PrepareRenderTexture()
    {
        if (!_setupDone)
            return;
        if (!(!_doTakeScreenshots && _shouldTakeScreenshots))
            return;
        if (!NativeWrapper.StartNewSequence(camera.targetTexture.width, camera.targetTexture.height))
            return;
        _startOfSequence = Time.realtimeSinceStartup;
        _doTakeScreenshots = true;
    }

    internal void CleanupRenderTexture()
    {
        if (!_setupDone)
            return;
        if (!(_doTakeScreenshots && !_shouldTakeScreenshots))
            return;
        if (!NativeWrapper.StopSequence())
            return;
        _doTakeScreenshots = false;
    }
}