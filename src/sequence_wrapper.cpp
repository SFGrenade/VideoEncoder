#include "main.h"

#include <stdexcept>

#include "_ffmpeg.h"
#include "common.h"

std::string get_ffmpeg_error_string( int e ) {
  std::array< char, AV_ERROR_MAX_STRING_SIZE > buffer;
  buffer.fill( 0 );
  return std::string( av_make_error_string( buffer.data(), AV_ERROR_MAX_STRING_SIZE, e ) );
}

namespace VEN {

uint64_t InputSequenceWrapper::_avio_read_pos = 0;

InputSequenceWrapper::InputSequenceWrapper( AVCodecID codec ) {
  int avret;
  _codec = avcodec_find_decoder( codec );
  if( !_codec ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _codec ({:d}) is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__, int( codec ) ) );
    throw std::runtime_error( "_codec is nullptr" );
  }
}

InputSequenceWrapper::~InputSequenceWrapper() {
  int avret;
  // cleanup
}

bool InputSequenceWrapper::read_png_bytes( uint8_t const* in_bytes, uint64_t in_bytes_size, std::vector< AVFrame* >& out_frames ) {
  int avret;

  uint8_t* av_buffer = nullptr;
  size_t av_buffer_size = 4096;

  // Create custom AVIO buffer
  av_buffer = static_cast< uint8_t* >( av_malloc( av_buffer_size ) );
  if( !av_buffer ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - buffer is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    return false;
  }

  // create format context
  _fmt_ctx = avformat_alloc_context();
  if( !_fmt_ctx ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _fmt_ctx is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    av_free( av_buffer );
    return false;
  }

  InputSequenceWrapper::_avio_read_pos = 0;

  std::pair< uint8_t const*, uint64_t > in_data_pair{ in_bytes, in_bytes_size };
  // get custom IO context
  AVIOContext* avio_ctx = avio_alloc_context(
      av_buffer,
      av_buffer_size,
      0,
      reinterpret_cast< void* >( &in_data_pair ),
      []( void* opaque, uint8_t* buf, int buf_size ) -> int {
        auto& bytes = *reinterpret_cast< std::pair< uint8_t const*, uint64_t >* >( opaque );
        int remaining = int( bytes.second - InputSequenceWrapper::_avio_read_pos );
        int to_copy = FFMIN( buf_size, remaining );
        memcpy( buf, bytes.first + InputSequenceWrapper::_avio_read_pos, to_copy );
        InputSequenceWrapper::_avio_read_pos += to_copy;
        return to_copy;
      },
      nullptr,
      nullptr );
  if( !avio_ctx ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avio_ctx is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    avformat_free_context( _fmt_ctx );
    av_free( av_buffer );
    return false;
  }

  // needed to indicate custom IO
  _fmt_ctx->pb = avio_ctx;
  _fmt_ctx->flags |= AVFMT_FLAG_CUSTOM_IO;

  // open input
  avret = avformat_open_input( &_fmt_ctx, nullptr, nullptr, nullptr );
  if( ( avret != 0 ) || ( !_fmt_ctx ) ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _fmt_ctx is nullptr or avformat_open_input returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    // already happens on failure
    // avformat_free_context( _fmt_ctx );
    // _fmt_ctx = nullptr;
    return false;
  }

  // get stream info
  avret = avformat_find_stream_info( _fmt_ctx, nullptr );
  if( avret != 0 ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avformat_find_stream_info returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  // technically not needed
  AVCodec const* stream_codec;
  // find best stream, should in theory be 0
  int stream_idx = av_find_best_stream( _fmt_ctx, AVMEDIA_TYPE_VIDEO, -1, -1, &stream_codec, 0 );
  if( ( stream_idx < 0 ) || ( !stream_codec ) ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - stream_idx is {:d} or stream_codec is {:p}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              stream_idx,
                              static_cast< void const* >( stream_codec ) ) );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  // get stream pointer
  // there should be only 1 stream, considering we send a single png frame over
  AVStream* stream = _fmt_ctx->streams[stream_idx];
  if( !stream ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - stream is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  // get codec context
  _codec_ctx = avcodec_alloc_context3( _codec );
  if( !_codec_ctx ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _codec_ctx is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  // copy params to context
  avret = avcodec_parameters_to_context( _codec_ctx, stream->codecpar );
  if( avret != 0 ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avformat_find_stream_info returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    avcodec_free_context( &_codec_ctx );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  // inform decoder about timebase and framerate
  _codec_ctx->pkt_timebase = stream->time_base;
  _codec_ctx->framerate = av_guess_frame_rate( _fmt_ctx, stream, NULL );

  // open decoder
  avret = avcodec_open2( _codec_ctx, _codec, NULL );
  if( avret != 0 ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avcodec_open2 returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    avcodec_free_context( &_codec_ctx );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  AVPacket* packet = av_packet_alloc();
  if( !packet ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - packet is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    avcodec_free_context( &_codec_ctx );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  AVFrame* frame = av_frame_alloc();
  if( !frame ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - frame is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    av_packet_free( &packet );
    avcodec_free_context( &_codec_ctx );
    av_free( avio_ctx->buffer );
    avio_context_free( &avio_ctx );
    avformat_close_input( &_fmt_ctx );
    return false;
  }

  while( av_read_frame( _fmt_ctx, packet ) >= 0 ) {
    if( packet->stream_index == stream_idx ) {
      if( avcodec_send_packet( _codec_ctx, packet ) == 0 ) {
        while( avcodec_receive_frame( _codec_ctx, frame ) == 0 ) {
          AVFrame* clone = av_frame_clone( frame );
          out_frames.push_back( clone );
        }
      }
    }
    av_packet_unref( packet );
  }

  av_frame_free( &frame );
  av_packet_free( &packet );
  avcodec_free_context( &_codec_ctx );
  avformat_close_input( &_fmt_ctx );
  av_free( avio_ctx->buffer );
  avio_context_free( &avio_ctx );
  return !out_frames.empty();
}

bool InputSequenceWrapper::read_raw_bytes( uint8_t const* in_bytes, uint64_t in_bytes_size, int width, int height, std::vector< AVFrame* >& out_frames ) {
  int avret;

  AVFrame* frame = av_frame_alloc();
  if( !frame ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - frame is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    return false;
  }

  frame->format = AV_PIX_FMT_RGB24;
  frame->width = width;
  frame->height = height;

  avret = av_frame_get_buffer( frame, 0 );
  if( avret != 0 ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - av_frame_get_buffer returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    av_frame_free( &frame );
    return false;
  }

  // Copy raw pixel data (RGB24)
  int rgb_stride = width * 3;
  for( int y = 0; y < height; ++y ) {
    // source texture is upside down
    memcpy( frame->data[0] + ( y * frame->linesize[0] ), in_bytes + ( ( height - y - 1 ) * rgb_stride ), rgb_stride );
  }

  out_frames.push_back( frame );

  return !out_frames.empty();
}

OutputSequenceWrapper::OutputSequenceWrapper( AVCodecID codec, int32_t width, int32_t height, std::filesystem::path const& file_path )
    : _codec( avcodec_find_encoder( codec ) ), _width( width ), _height( height ), _file_path( file_path ) {
  int avret;

  if( !_codec ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _codec ({:d}) is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__, int( codec ) ) );
    throw std::runtime_error( "_codec is nullptr" );
  }

  _codec_ctx = avcodec_alloc_context3( _codec );
  if( !_codec_ctx ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _codec_ctx ({:d}) is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__, int( codec ) ) );
    throw std::runtime_error( "_codec_ctx is nullptr" );
  }
  _codec_ctx->width = _width;
  _codec_ctx->height = _height;
  _codec_ctx->pix_fmt = AV_PIX_FMT_YUV420P;
  _codec_ctx->time_base = AVRational{ 1, 50 };  // sure why not
  _codec_ctx->framerate = AVRational{ 50, 1 };  // sure why not
  _codec_ctx->bit_rate = 1'000'000;             // sure why not

  AVDictionary* opts = nullptr;
  av_dict_set( &opts, "deadline", "realtime", 0 );
  av_dict_set( &opts, "cpu-used", "16", 0 );
  av_dict_set( &opts, "speed", "16", 0 );
  av_dict_set( &opts, "quality", "realtime", 0 );
  av_dict_set( &opts, "threads", "4", 0 );

  avret = avcodec_open2( _codec_ctx, _codec, &opts );
  if( 0 > avret ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avformat_alloc_output_context2 returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    throw std::runtime_error( "Failed to open VP8 codec" );
  }

  avret = avformat_alloc_output_context2( &_fmt_ctx, nullptr, "matroska", _file_path.string().c_str() );
  if( ( 0 > avret ) || ( !_fmt_ctx ) ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avformat_alloc_output_context2 returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    throw std::runtime_error( "avformat_alloc_output_context2 error" );
  }

  _stream = avformat_new_stream( _fmt_ctx, _codec );
  if( !_stream ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - _stream is nullptr!", static_cast< void* >( this ), __FUNCTION__, __LINE__ ) );
    throw std::runtime_error( "_stream is nullptr" );
  }
  _stream->time_base = _codec_ctx->time_base;
  _stream->r_frame_rate = _codec_ctx->framerate;
  avret = avcodec_parameters_from_context( _stream->codecpar, _codec_ctx );
  if( 0 > avret ) {
    printInFile( fmt::format( "{:p}:{:s}:{:d} - avcodec_parameters_from_context returned {:d}: {:s}!",
                              static_cast< void* >( this ),
                              __FUNCTION__,
                              __LINE__,
                              avret,
                              get_ffmpeg_error_string( avret ) ) );
    throw std::runtime_error( "avcodec_parameters_from_context error" );
  }

  if( !( _fmt_ctx->oformat->flags & AVFMT_NOFILE ) ) {
    if( avio_open( &_fmt_ctx->pb, file_path.string().c_str(), AVIO_FLAG_WRITE ) < 0 )
      throw std::runtime_error( "Could not open output file" );
  }

  if( avformat_write_header( _fmt_ctx, nullptr ) < 0 )
    throw std::runtime_error( "Failed to write header" );
}

OutputSequenceWrapper::~OutputSequenceWrapper() {
  int avret;

  {
    // write null packet
    AVPacket* pkt = av_packet_alloc();
    if( avcodec_send_frame( _codec_ctx, nullptr ) == 0 ) {
      while( avcodec_receive_packet( _codec_ctx, pkt ) == 0 ) {
        av_interleaved_write_frame( _fmt_ctx, pkt );
        av_packet_unref( pkt );
      }
    }
    av_packet_free( &pkt );
  }

  av_write_trailer( _fmt_ctx );

  // cleanup
  if( !( _fmt_ctx->oformat->flags & AVFMT_NOFILE ) )
    avio_closep( &_fmt_ctx->pb );
  avformat_free_context( _fmt_ctx );
  _fmt_ctx = nullptr;
  avcodec_free_context( &_codec_ctx );
}

bool OutputSequenceWrapper::write_vp8_frames( std::vector< AVFrame* > const& frames ) {
  int avret;

  AVPacket* pkt = av_packet_alloc();

  for( AVFrame* frame : frames ) {
    frame->pts = _frame_counter++;
    if( avcodec_send_frame( _codec_ctx, frame ) < 0 )
      continue;

    while( avcodec_receive_packet( _codec_ctx, pkt ) == 0 ) {
      av_packet_rescale_ts( pkt, _codec_ctx->time_base, _stream->time_base );
      pkt->stream_index = _stream->index;
      av_interleaved_write_frame( _fmt_ctx, pkt );
      av_packet_unref( pkt );
    }
  }

  av_packet_free( &pkt );

  return true;
}

int32_t OutputSequenceWrapper::get_width() const {
  return _width;
}

int32_t OutputSequenceWrapper::get_height() const {
  return _height;
}

}  // namespace VEN
