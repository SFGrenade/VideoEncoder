using System;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace VideoEncoder;

public class ScreenshotMb : MonoBehaviour
{
    public string dir;
    public Camera camera;

    private bool _setupDone = false;
    private bool _shouldTakeScreenshots = false;
    private bool _doTakeScreenshots = false;
    private bool _screenShotTimingNow = false;
    private double _startOfSequence = 0.0;

    [UsedImplicitly]
    private void FixedUpdate()
    {
        _screenShotTimingNow = true;
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

        // this is true when _doTakeScreenshots is true and then either CapRecordingToFixedUpdate is false or it also needs _screenShotTimingNow to be true
        if (_doTakeScreenshots && ((!VideoEncoder.GlobalSettings.CapRecordingToFixedUpdate) || _screenShotTimingNow))
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
                _screenShotTimingNow = false;
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