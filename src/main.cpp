#include "main.h"

#include <algorithm>
#include <chrono>
#include <filesystem>

#include "_ffmpeg.h"
#include "common.h"

bool Init( char const* mod_dir, char const* save_dir, LogCallback logging_callback ) {
  ::GS::mod_dir = std::filesystem::path( mod_dir );
  ::GS::save_dir = std::filesystem::path( save_dir );
  setCallback( logging_callback );
  printInFile( fmt::format( "{:s}:{:d} - Initializing library with ( {:?}, {:?} )", __FUNCTION__, __LINE__, mod_dir, save_dir ) );

  av_log_set_level( AV_LOG_ERROR );

  printInFile( fmt::format( "{:s}:{:d} - Library initialized!", __FUNCTION__, __LINE__ ) );
  return true;
}

bool SetFileExtension( char const* file_extension ) {
  printInFile( fmt::format( "{:s}( file_extension={:?} )", __FUNCTION__, file_extension ) );

  ::GS::file_extension = std::string( file_extension );

  printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

bool SetCodecOption( char const* key, char const* value ) {
  printInFile( fmt::format( "{:s}( key={:?}, value={:?} )", __FUNCTION__, key, value ) );

  std::string keyStr( key );
  std::string valueStr( value );
  auto ret = ::GS::codec_options.try_emplace( keyStr, valueStr );

  printInFile( fmt::format( "{:s}:{:d}~ => {}", __FUNCTION__, __LINE__, ret.second ) );
  return ret.second;
}

bool SetMediaOption( char const* key, char const* value ) {
  printInFile( fmt::format( "{:s}( key={:?}, value={:?} )", __FUNCTION__, key, value ) );

  std::string keyStr( key );
  std::string valueStr( value );
  auto ret = ::GS::media_options.try_emplace( keyStr, valueStr );

  printInFile( fmt::format( "{:s}:{:d}~ => {}", __FUNCTION__, __LINE__, ret.second ) );
  return ret.second;
}

bool Deinit() {
  printInFile( fmt::format( "{:s}:{:d} - Deinitializing library...", __FUNCTION__, __LINE__ ) );

  while( !::GS::input_threads.empty() ) {
    std::shared_ptr< InputThread > thread = ::GS::input_threads.front();
    ::GS::input_threads.pop_front();
    if( thread ) {
      thread.reset();
    }
  }

  printInFile( fmt::format( "{:s}:{:d} - Library deinitialized!", __FUNCTION__, __LINE__ ) );
  return true;
}

std::string getISOCurrentTimestamp() {
  return fmt::format( "{:%FT%TZ}", std::chrono::system_clock::now() );
}

bool StartNewSequence( int32_t width, int32_t height ) {
  printInFile( fmt::format( "{:s}( width={:d}, height={:d} )", __FUNCTION__, width, height ) );
  // todo: fixme: change how the name is generated
  std::string filename = fmt::format( "Recording_{:d}_{:s}.{:s}", ::GS::sequence_index, getISOCurrentTimestamp(), ::GS::file_extension );
  std::replace( filename.begin(), filename.end(), ':', '-' );
  std::filesystem::path output_filename = ::GS::mod_dir / filename;
  ::GS::sequence_index++;

  printInFile( fmt::format( "Opening new sequence: {:s}", output_filename.string() ) );

  if( !::GS::input_threads.empty() ) {
    ::GS::input_threads.back()->end_input_thread();
  }

  OutputSequenceWrapper* out_wrapper = new OutputSequenceWrapper( AVCodecID::AV_CODEC_ID_VP8, width, height, output_filename );
  ::GS::input_threads.emplace_back( new InputThread( out_wrapper ) );

  printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

bool SendPngBytes( uint8_t const* bytes, int32_t length ) {
  if( ::GS::input_threads.back() ) {
    ::GS::input_threads.back()->receive_bytes( bytes, length );
    return true;
  }
  return false;
}

bool SendRawBytes( uint8_t const* bytes, int32_t length, int width, int height, double timestamp ) {
  if( ::GS::input_threads.back() ) {
    ::GS::input_threads.back()->receive_bytes( bytes, length, width, height, timestamp );
    return true;
  }
  return false;
}

bool StopSequence() {
  printInFile( fmt::format( "{:s}()", __FUNCTION__ ) );

  if( ::GS::input_threads.back() ) {
    ::GS::input_threads.back()->end_input_thread();
  }

  printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

uint64_t GetSizeOfAllQueues() {
  uint64_t ret = 0;
  for( auto const& item : ::GS::input_threads ) {
    ret += item->get_queue_size();
  }
  return ret;
}
