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
    private RenderTexture _activeRenderTexture = null;

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
        if (!NativeWrapper.StartNewSequence(Screen.width, Screen.height))
            return;
        _doTakeScreenshots = true;

        camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);

        _activeRenderTexture = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;
    }

    private void DoScreenshot(RenderTexture textureToSave)
    {
        Texture2D image = new Texture2D(textureToSave.width, textureToSave.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, textureToSave.width, textureToSave.height), 0, 0);
        image.Apply();

        byte[] png_bytes = image.EncodeToPNG();
        UObject.DestroyImmediate(image);

        NativeWrapper.SendPngBytes(png_bytes);
    }

    private void CleanupRenderTexture()
    {
        if (!NativeWrapper.StopSequence())
            return;
        _doTakeScreenshots = false;

        RenderTexture.active = _activeRenderTexture;
        camera.targetTexture = null;
    }

    private static TextureFormat RenderToRegularFormat(RenderTextureFormat a)
    {
        switch (a)
        {
            case RenderTextureFormat.ARGB32:
                return TextureFormat.ARGB32;
            // case RenderTextureFormat.Depth:
            //     return TextureFormat.Depth;
            case RenderTextureFormat.ARGBHalf:
                return TextureFormat.RGBAHalf; // switch
            // case RenderTextureFormat.Shadowmap:
            //     return TextureFormat.Shadowmap;
            case RenderTextureFormat.RGB565:
                return TextureFormat.RGB565;
            case RenderTextureFormat.ARGB4444:
                return TextureFormat.ARGB4444;
            // case RenderTextureFormat.ARGB1555:
            //     return TextureFormat.ARGB1555;
            // case RenderTextureFormat.Default:
            //     return TextureFormat.Default;
            // case RenderTextureFormat.ARGB2101010:
            //     return TextureFormat.ARGB2101010;
            // case RenderTextureFormat.DefaultHDR:
            //     return TextureFormat.DefaultHDR;
            case RenderTextureFormat.ARGB64:
                return TextureFormat.RGBA64; // switch
            case RenderTextureFormat.ARGBFloat:
                return TextureFormat.RGBAFloat; // switch
            case RenderTextureFormat.RGFloat:
                return TextureFormat.RGFloat;
            case RenderTextureFormat.RGHalf:
                return TextureFormat.RGHalf;
            case RenderTextureFormat.RFloat:
                return TextureFormat.RFloat;
            case RenderTextureFormat.RHalf:
                return TextureFormat.RHalf;
            case RenderTextureFormat.R8:
                return TextureFormat.R8;
            // case RenderTextureFormat.ARGBInt:
            //     return TextureFormat.ARGBInt;
            // case RenderTextureFormat.RGInt:
            //     return TextureFormat.RGInt;
            // case RenderTextureFormat.RInt:
            //     return TextureFormat.RInt;
            case RenderTextureFormat.BGRA32:
                return TextureFormat.BGRA32;
            // case RenderTextureFormat.RGB111110Float:
            //     return TextureFormat.RGB111110Float;
            case RenderTextureFormat.RG32:
                return TextureFormat.RG32;
            // case RenderTextureFormat.RGBAUShort:
            //     return TextureFormat.RGBAUShort;
            case RenderTextureFormat.RG16:
                return TextureFormat.RG16;
            // case RenderTextureFormat.BGRA10101010_XR:
            //     return TextureFormat.BGRA10101010_XR;
            // case RenderTextureFormat.BGR101010_XR:
            //     return TextureFormat.BGR101010_XR;
            case RenderTextureFormat.R16:
                return TextureFormat.R16;
        }

        return TextureFormat.ARGB32;
    }
}