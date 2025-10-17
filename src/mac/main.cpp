#include "../main.h"

#include <filesystem>

bool Init( char const* base_dir ) {
  g_base_dir = std::filesystem::path( base_dir );
  openFile( g_base_dir, true );
  printInFile( "Initializing library..." );

  if( _RegisterSignalCallbacks() <= 0 ) {
    printInFile( "Error registering signal callbacks!" );
    return false;
  }

  // more code here

  printInFile( "Library initialized!" );
  return true;
}

bool Deinit() {
  printInFile( "Deinitializing library..." );

  // more code here

  closeFile();

  return true;
}

bool StartNewSequence( int32_t width, int32_t height ) {
  printInFile( "StartNewSequence( width: %d, height: %d ) - MacOS", width, height );

  printInFile( "StartNewSequence is unsupported on MacOS" );

  return false;
}

bool SendExrBytes( uint8_t const* bytes, int32_t length ) {
  printInFile( "SendExrBytes( bytes: %p, length: %d ) - MacOS", bytes, length );

  printInFile( "SendExrBytes is unsupported on MacOS" );

  return false;
}

bool SendPngBytes( uint8_t const* bytes, int32_t length ) {
  printInFile( "SendPngBytes( bytes: %p, length: %d ) - MacOS", bytes, length );

  printInFile( "SendPngBytes is unsupported on MacOS" );

  return false;
}

bool SendTgaBytes( uint8_t const* bytes, int32_t length ) {
  printInFile( "SendTgaBytes( bytes: %p, length: %d ) - MacOS", bytes, length );

  printInFile( "SendTgaBytes is unsupported on MacOS" );

  return false;
}

bool StopSequence() {
  printInFile( "StopSequence() - MacOS" );

  printInFile( "StopSequence is unsupported on MacOS" );

  return false;
}
