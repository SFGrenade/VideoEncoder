using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VideoEncoder;

public class GlobalSettings
{
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