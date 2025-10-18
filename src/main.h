#ifndef VIDEOENCODERNATIVE_MAIN_H_
#define VIDEOENCODERNATIVE_MAIN_H_

#include <atomic>
#include <cstdint>
#include <filesystem>
#include <vector>

#include "_ffmpeg.h"
#include "common.h"

void _SignalCallback( int const sigNum );
int _RegisterSignalCallbacks();

#if defined( CM_Windows )
#elif defined( CM_MacOS )
#elif defined( CM_Linux )
#endif

namespace VEN {
static std::filesystem::path g_mod_dir;
static std::filesystem::path g_save_dir;
static std::atomic_uint64_t g_sequence_index = 0;

class InputSequenceWrapper {
  public:
  InputSequenceWrapper( AVCodecID codec );
  InputSequenceWrapper( InputSequenceWrapper& ) = delete;
  ~InputSequenceWrapper();

  bool read_png_bytes( std::vector< uint8_t > const& in_bytes, std::vector< AVFrame* >& out_frames );

  private:
  AVCodec const* _codec = nullptr;

  AVCodecContext* _codec_ctx = nullptr;
  AVFormatContext* _fmt_ctx = nullptr;
};

class OutputSequenceWrapper {
  public:
  OutputSequenceWrapper( AVCodecID codec, int32_t width, int32_t height, std::filesystem::path const& file_path );
  OutputSequenceWrapper( OutputSequenceWrapper& ) = delete;
  ~OutputSequenceWrapper();

  bool write_vp8_frames( std::vector< AVFrame* > const& frames );

  private:
  AVCodec const* _codec = nullptr;
  int32_t _width = 0;
  int32_t _height = 0;
  std::filesystem::path _file_path;

  AVCodecContext* _codec_ctx = nullptr;
  AVFormatContext* _fmt_ctx = nullptr;
};

static OutputSequenceWrapper* g_out_wrapper = nullptr;

}  // namespace VEN

extern "C" {
EXPORT bool Init( char const* mod_dir, char const* save_dir, LogCallback logging_callback );
EXPORT bool Deinit();
EXPORT bool StartNewSequence( int32_t width, int32_t height );
EXPORT bool SendPngBytes( uint8_t const* bytes, int32_t length );
EXPORT bool StopSequence();
}

#endif  // VIDEOENCODERNATIVE_MAIN_H_
