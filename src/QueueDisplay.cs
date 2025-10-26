using System;
using System.Collections;
using System.Globalization;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace VideoEncoder;

public class QueueDisplay : MonoBehaviour
{
    private const float UpdateInterval = 1f/8f;

    private string _displayText = "";
    private readonly GUIStyle _textStyle = new GUIStyle(GUIStyle.none);
    private Font _monoFont;

    [UsedImplicitly]
    private void Start()
    {
        _monoFont = Font.CreateDynamicFontFromOSFont(new string[]
        {
            // Windows
            "Consolas",
            // Mac
            "Menlo",
            // Linux
            "Courier New",
            "DejaVu Mono"
        }, 15);
        _textStyle.font = _monoFont;
        _textStyle.normal.textColor = Color.white;
        _textStyle.alignment = TextAnchor.UpperRight;
        _textStyle.padding = new RectOffset(5, 5, 5, 5);
    }

    [UsedImplicitly]
    private void OnEnable()
    {
        StartCoroutine(CalcTimedFps());
    }

    private IEnumerator CalcTimedFps()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSecondsRealtime(UpdateInterval);

            GenerateText();
        }
    }

    private void GenerateText()
    {
        if (ScreenshotMb.Instance.IsRecording())
        {
            _displayText = "[RECORDING: ON]";
        }
        else
        {
            _displayText = "[RECORDING: OFF]";
        }
        _displayText += $"\nFrames in queue(s): {NativeWrapper.GetSizeOfAllQueues()}";
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), _displayText, _textStyle);
    }
}