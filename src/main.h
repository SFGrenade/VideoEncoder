#ifndef VIDEOENCODERNATIVE_MAIN_H_
#define VIDEOENCODERNATIVE_MAIN_H_

#include <array>
#include <atomic>
#include <cstdint>
#include <filesystem>
#include <vector>

#include "_ffmpeg.h"
#include "common.h"

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

  bool read_png_bytes( uint8_t const* in_bytes, uint64_t in_bytes_size, std::vector< AVFrame* >& out_frames );
  bool read_raw_bytes( uint8_t const* in_bytes, uint64_t in_bytes_size, int width, int height, std::vector< AVFrame* >& out_frames );

  private:
  AVCodec const* _codec = nullptr;

  AVCodecContext* _codec_ctx = nullptr;
  AVFormatContext* _fmt_ctx = nullptr;

  static uint64_t _avio_read_pos;
};

class OutputSequenceWrapper {
  public:
  OutputSequenceWrapper( AVCodecID codec, int32_t width, int32_t height, std::filesystem::path const& file_path );
  OutputSequenceWrapper( OutputSequenceWrapper& ) = delete;
  ~OutputSequenceWrapper();

  bool write_vp8_frames( std::vector< AVFrame* > const& frames, double timestamp );

  int32_t get_width() const;
  int32_t get_height() const;

  uint64_t _frame_counter = 0;

  private:
  AVCodec const* _codec = nullptr;
  int32_t _width = 0;
  int32_t _height = 0;
  std::filesystem::path _file_path;

  AVCodecContext* _codec_ctx = nullptr;
  AVFormatContext* _fmt_ctx = nullptr;
  AVStream* _stream = nullptr;
};

static OutputSequenceWrapper* g_out_wrapper = nullptr;

}  // namespace VEN

extern "C" {
EXPORT bool CDECL Init( char const* mod_dir, char const* save_dir, LogCallback logging_callback );
EXPORT bool CDECL Deinit();
EXPORT bool CDECL StartNewSequence( int32_t width, int32_t height );
EXPORT bool CDECL SendPngBytes( uint8_t const* bytes, int32_t length );
EXPORT bool CDECL SendRawBytes( uint8_t const* bytes, int32_t length, int width, int height, double timestamp );
EXPORT bool CDECL StopSequence();
}

#endif  // VIDEOENCODERNATIVE_MAIN_H_
