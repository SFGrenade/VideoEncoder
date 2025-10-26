using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VideoEncoder;

public class GlobalSettings
{
    public KeyCode StartStopKey = KeyCode.F12;
    public bool ShowRecordingUi = true;
    public bool StartRecordingOnStart = false;
    public bool CapRecordingToFixedUpdate = true;
    public string FileExtension = "mkv";
    public float RenderResolutionScale = 0.5f;
    public int RenderFpsDenumerator = 2;

    public Dictionary<string, string> Codec = new()
    {
        { "deadline", "realtime" },
        { "cpu-used", "16" },
        { "speed", "16" },
        { "quality", "realtime" },
        { "threads", "4" },
    };

    public Dictionary<string, string> Container = new();
}