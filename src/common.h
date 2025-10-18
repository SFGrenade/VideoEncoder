#ifndef VIDEOENCODERNATIVE_COMMON_H_
#define VIDEOENCODERNATIVE_COMMON_H_

#include "platform.h"

#if defined( CM_Windows )
#define EXPORT __declspec( dllexport )
#define IMPORT __declspec( dllimport )
#elif defined( CM_MacOS ) || defined( CM_Linux )
#define EXPORT __attribute__( ( visibility( "default" ) ) )
#define IMPORT
#else
#define EXPORT
#define IMPORT
#pragma warning Unknown dynamic link import / export semantics.
#endif

#include <fmt/base.h>
#include <fmt/format.h>
#include <fmt/ranges.h>
#include <fmt/chrono.h>
#include <fmt/std.h>
#include <string>
#include <utility>

typedef void ( *LogCallback )( char const *message );
static LogCallback g_log_message = nullptr;

void setCallback( LogCallback callback );
void printInFile( std::string const &msg );
// for some reason fmt shits itself when doing this
// template < typename... Args >
// void printInFile( std::string const &format, Args... args ) {
//   printInFile( fmt::format( format, std::forward< Args >( args )... ) );
//   // printInFile( fmt::format( format, args... ) );
// }

#if defined( CM_Windows )
#include <Windows.h>
extern "C" {
BOOL WINAPI DllMain( HINSTANCE const dllModHandle, DWORD const callReason, LPVOID const reserved );
// BOOL WINAPI _DllMainCRTStartup( HINSTANCE const dllModHandle, DWORD const callReason, LPVOID const reserved );
}
#endif

#endif  // VIDEOENCODERNATIVE_COMMON_H_
