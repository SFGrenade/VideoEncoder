#ifndef VIDEOENCODERNATIVE_PLATFORM_H_
#define VIDEOENCODERNATIVE_PLATFORM_H_

#if defined( WIN32 ) || defined( _WIN32 ) || defined( __WIN32__ ) || defined( __NT__ )
// define something for Windows (32-bit and 64-bit, this part is common)
#define CM_Windows
#define CDECL __cdecl
#ifdef _WIN64
// define something for Windows (64-bit only)
#else
// define something for Windows (32-bit only)
#endif
#elif __APPLE__
#define CM_MacOS
#define CDECL __cdecl
#include <TargetConditionals.h>
#if TARGET_IPHONE_SIMULATOR
// iOS, tvOS, or watchOS Simulator
#elif TARGET_OS_MACCATALYST
// Mac's Catalyst (ports iOS API into Mac, like UIKit).
#elif TARGET_OS_IPHONE
// iOS, tvOS, or watchOS device
#elif TARGET_OS_MAC
// Other kinds of Apple platforms
#else
#error "Unknown Apple platform"
#endif
#elif __ANDROID__
// Below __linux__ check should be enough to handle Android,
// but something may be unique to Android.
#define CM_Android
#elif __linux__
// linux
#define CM_Linux
#define CDECL __attribute__((__cdecl__))
#elif __unix__  // all unices not caught above
// Unix
#define CM_Linux
#define CDECL __attribute__((__cdecl__))
#elif defined( _POSIX_VERSION )
// POSIX
#define CM_Linux
#define CDECL __attribute__((__cdecl__))
#else
#error "Unknown compiler"
#endif

#endif  // VIDEOENCODERNATIVE_PLATFORM_H_
