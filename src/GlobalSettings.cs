using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VideoEncoder;

public class GlobalSettings
{
    public KeyCode StartStopKey = KeyCode.F12;
    /*
     * todo: add some kinda display in the top-right corner like:
     * [not recording] vs [RECORDING!]
     * {:d} frames in render queue(s)
     */
    public bool ShowRecordingUi = true;
    public bool StartRecordingOnStart = false;
    public bool CapRecordingToFixedUpdate = false;
    public string FileExtension = "mkv";

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