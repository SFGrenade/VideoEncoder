# VideoEncoder

A mod for the game Hollow Knight.

## Settings

The settings for the recording can be adjusted via the `VideoEncoder.GlobalSettings.json` file in the save data folder.

They are as following:
- `StartStopKey`: The [KeyCode](<https://docs.unity3d.com/2020.2/Documentation/ScriptReference/KeyCode.html#Properties>) that starts and stops the recording. Default is `F12` (`293`).
- `ShowRecordingUi`: This, by default `true`, will show a small UI in the top-right corner with the status of the recording(s).
- `StartRecordingOnStart`: When `true` (default is `false`), this will start a recording as soon as the game starts.
- `CapRecordingToFixedUpdate`: When `true` (default), the mod will attempt to get frames based on `FixedUpdate` (in Hollow Knight 50 Hz), resulting in a more stable output video.
- `FileExtension`: The file extension the video should have (default is `"mkv"`, also possible is `"webm"`).
- `RenderResolutionScale`: The scale of which the rendered video should be. E.g. `0.5` (default) means that with a screen size of `1920x1080`, the resulting video would be `960x540`.
- `RenderFpsDenumerator`: This says which Nth frame to save to video. E.g. `2` (default) means that every other frame will be sent into the video conversion, `1` would mean every frame, `3` every third.
- `Codec`: A list of key-value pairs where the key names and respective value ranges can be taken from the file `ffmpeg_vp8.txt` (and for `Delay`/`Threads` use `ffmpeg.txt`) on the `C++` branch of this repository.
- `Container`: A list of key-value pairs where the key names and respective value ranges can be taken from the file `ffmpeg_matroska.txt` on the `C++` branch of this repository.

# EUPL
                      Copyright (c) 2025 SFGrenade
                      Licensed under the EUPL-1.2
https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
