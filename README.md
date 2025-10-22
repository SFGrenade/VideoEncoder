# VideoEncoder

A mod for the game Hollow Knight.

## Settings

The settings for the recording can be adjusted via the `VideoEncoder.GlobalSettings.json` file in the save data folder.

They are as following:
- `StartStopKey`: The [KeyCode](<https://docs.unity3d.com/2020.2/Documentation/ScriptReference/KeyCode.html#Properties>) that starts and stops the recording.
- `ShowRecordingUi`: In the future, this, by default `true`, will show a small UI in the top-right corner with the status of the recording(s).
- `StartRecordingOnStart`: When `true` (default is `false`), this will start a recording as soon as the game starts.
- `CapRecordingToFixedUpdate`: When `true` (default is `false`), the mod will attempt to get frames after every `FixedUpdate` (in Hollow Knight 50 Hz), resulting in a more stable output video.
- `FileExtension`: The file extension the video should have (default is `"mkv"`, also possible is `"webm"`).
- `Codec`: A list of key-value pairs where the key names and respective value ranges can be taken from the file `ffmpeg_vp8.txt` (and for `Delay`/`Threads` use `ffmpeg.txt`) on the `C++` branch of this repository.
- `Container`: A list of key-value pairs where the key names and respective value ranges can be taken from the file `ffmpeg_matroska.txt` on the `C++` branch of this repository.

# EUPL
                      Copyright (c) 2025 SFGrenade
                      Licensed under the EUPL-1.2
https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
