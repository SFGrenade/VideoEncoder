using System;
using System.IO;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace VideoEncoder;

public class ScreenshotMb : MonoBehaviour
{
    public string dir;
    public Camera camera;

    private bool _shouldTakeScreenshots = false;
    private bool _doTakeScreenshots = false;

    [UsedImplicitly]
    private void Update()
    {
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

    private void PrepareRenderTexture()
    {
        if (!NativeWrapper.StartNewSequence(camera.targetTexture.width, camera.targetTexture.height))
            return;
        _doTakeScreenshots = true;
    }

    private void DoScreenshot(RenderTexture textureToSave)
    {
        Texture2D image = new Texture2D(textureToSave.width, textureToSave.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, textureToSave.width, textureToSave.height), 0, 0);
        image.Apply();

        //byte[] png_texture_bytes = image.EncodeToPNG();
        byte[] raw_texture_bytes = image.GetRawTextureData();
        UObject.DestroyImmediate(image);

        //NativeWrapper.SendPngBytes(png_texture_bytes);
        NativeWrapper.SendRawBytes(raw_texture_bytes, textureToSave.width, textureToSave.height);
    }

    private void CleanupRenderTexture()
    {
        if (!NativeWrapper.StopSequence())
            return;
        _doTakeScreenshots = false;
    }
}