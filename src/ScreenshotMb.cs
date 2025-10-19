using System;
using System.IO;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UObject = UnityEngine.Object;

namespace VideoEncoder;

public class ScreenshotMb : MonoBehaviour
{
    public string dir;
    public Camera camera;

    private bool _setupDone = false;
    private bool _shouldTakeScreenshots = false;
    private bool _doTakeScreenshots = false;
    private double _startOfSequence = 0.0;

    [UsedImplicitly]
    private void Update()
    {
        if (!_setupDone && GameCameras.instance != null && GameCameras.instance.mainCamera != null)
        {
            _setupDone = true;
            RenderTexture prevTarget = GetComponent<Camera>().targetTexture; 
            GetComponent<Camera>().CopyFrom(GameCameras.instance.mainCamera);
            GetComponent<Camera>().targetTexture = prevTarget; 
        }

        if (HeroController.instance != null)
        {
            transform.position = new Vector3(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y, -38.1f);
        }
        else if (GameCameras.instance != null && GameCameras.instance.mainCamera != null)
        {
            transform.position = new Vector3(GameCameras.instance.mainCamera.transform.position.x, GameCameras.instance.mainCamera.transform.position.y, -38.1f);
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleRecording();
        }
    }

    public void ToggleRecording()
    {
        _shouldTakeScreenshots = !_shouldTakeScreenshots;
    }

    [UsedImplicitly]
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        Graphics.Blit(src, dest);

        if (_doTakeScreenshots)
        {
            DoScreenshot(dest);
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
        if (!(!_doTakeScreenshots && _shouldTakeScreenshots))
            return;
        if (!NativeWrapper.StartNewSequence(camera.targetTexture.width, camera.targetTexture.height))
            return;
        _startOfSequence = Time.realtimeSinceStartup;
        _doTakeScreenshots = true;
    }

    private void DoScreenshot(RenderTexture textureToSave)
    {
        double now = Time.realtimeSinceStartup;
        AsyncGPUReadback.Request(textureToSave, 0, TextureFormat.RGB24, request =>
        {
            if (request.hasError || !_doTakeScreenshots)
                return;

            byte[] data = request.GetData<byte>().ToArray();
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    NativeWrapper.SendRawBytes((IntPtr)ptr, data.Length, textureToSave.width, textureToSave.height, now - _startOfSequence);
                }
            }
        });
    }

    internal void CleanupRenderTexture()
    {
        if (!(_doTakeScreenshots && !_shouldTakeScreenshots))
            return;
        if (!NativeWrapper.StopSequence())
            return;
        _doTakeScreenshots = false;
    }
}