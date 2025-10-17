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

#define MSG_SIZE 256

#include <cstdio>
#include <filesystem>
#include <utility>

extern FILE *logFile;
void openFile( std::filesystem::path const& log_file, bool truncate );
void closeFile();
void printInFile( char const *msg );
template < typename... Args >
void printInFile( char const *format, Args... args ) {
  if( logFile != nullptr ) {
    fprintf( logFile, format, std::forward< Args >( args )... );
    fprintf( logFile, "\n" );
    fflush( logFile );
  }
}

#if defined( CM_Windows )
#include <Windows.h>

extern "C" {
BOOL WINAPI DllMain( HINSTANCE const dllModHandle, DWORD const callReason, LPVOID const reserved );
// BOOL WINAPI _DllMainCRTStartup( HINSTANCE const dllModHandle, DWORD const callReason, LPVOID const reserved );
}
#endif

#endif  // VIDEOENCODERNATIVE_COMMON_H_
