#include "main.h"

#include <stdexcept>

#include "_ffmpeg.h"

namespace VEN {

InputSequenceWrapper::InputSequenceWrapper( AVCodecID codec ) {
  int avret;
  _codec = avcodec_find_encoder( codec );
  if( !_codec ) {
    printInFile( "%p:%s:%d - _codec (%d) is nullptr!", this, __FUNCTION__, __LINE__, codec );
    throw std::runtime_error( "_codec is nullptr" );
  }

  _codec_ctx = avcodec_alloc_context3( _codec );
  if( !_codec_ctx ) {
    printInFile( "%p:%s:%d - _codec_ctx (%d) is nullptr!", this, __FUNCTION__, __LINE__, codec );
    throw std::runtime_error( "_codec_ctx is nullptr" );
  }
}

InputSequenceWrapper::~InputSequenceWrapper() {
  int avret;
  // cleanup
  avcodec_free_context( &_codec_ctx );
}

bool InputSequenceWrapper::read_png_bytes( std::vector< uint8_t > const& in_bytes, std::vector< AVFrame* >& out_frames ) {
  int avret;
  return false;
}

OutputSequenceWrapper::OutputSequenceWrapper( AVCodecID codec, int32_t width, int32_t height, std::filesystem::path const& file_path )
    : _width( width ), _height( height ), _file_path( file_path ) {
  int avret;
  _codec = avcodec_find_encoder( codec );
  if( !_codec ) {
    printInFile( "%p:%s:%d - _codec (%d) is nullptr!", this, __FUNCTION__, __LINE__, codec );
    throw std::runtime_error( "_codec is nullptr" );
  }

  _codec_ctx = avcodec_alloc_context3( _codec );
  if( !_codec_ctx ) {
    printInFile( "%p:%s:%d - _codec_ctx (%d) is nullptr!", this, __FUNCTION__, __LINE__, codec );
    throw std::runtime_error( "_codec_ctx is nullptr" );
  }

  avret = avformat_alloc_output_context2( &_fmt_ctx, nullptr, nullptr, _file_path.string().c_str() );
  if( ( 0 > avret ) || ( !_fmt_ctx ) ) {
    printInFile( "%p:%s:%d - avformat_alloc_output_context2 returned %d!", this, __FUNCTION__, __LINE__, codec );
    throw std::runtime_error( "avformat_alloc_output_context2 error" );
  }
}

OutputSequenceWrapper::~OutputSequenceWrapper() {
  int avret;
  // maybe write trailer

  // cleanup
  avformat_free_context( _fmt_ctx );
  _fmt_ctx = nullptr;
  avcodec_free_context( &_codec_ctx );
}

bool OutputSequenceWrapper::write_vp8_frames( std::vector< AVFrame* > const& frames ) {
  int avret;
  return false;
}

}  // namespace VEN
