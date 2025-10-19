#include "main.h"

#include <filesystem>

#include "_ffmpeg.h"
#include "common.h"

bool Init( char const* mod_dir, char const* save_dir, LogCallback logging_callback ) {
  ::VEN::g_mod_dir = std::filesystem::path( mod_dir );
  ::VEN::g_save_dir = std::filesystem::path( save_dir );
  setCallback( logging_callback );
  printInFile( fmt::format( "{:s}:{:d} - Initializing library with ( '{:s}', '{:s}' )", __FUNCTION__, __LINE__, mod_dir, save_dir ) );

  av_log_set_level( AV_LOG_ERROR );

  printInFile( fmt::format( "{:s}:{:d} - Library initialized!", __FUNCTION__, __LINE__ ) );
  return true;
}

bool Deinit() {
  printInFile( fmt::format( "{:s}:{:d} - Deinitializing library...", __FUNCTION__, __LINE__ ) );

  printInFile( fmt::format( "{:s}:{:d} - Library deinitialized!", __FUNCTION__, __LINE__ ) );
  return true;
}

bool StartNewSequence( int32_t width, int32_t height ) {
  printInFile( fmt::format( "{:s}( width={:d}, height={:d} )", __FUNCTION__, width, height ) );
  std::filesystem::path output_filename = ::VEN::g_mod_dir / ( std::to_string( ::VEN::g_sequence_index ) + std::string( ".mkv" ) );
  ::VEN::g_sequence_index = ::VEN::g_sequence_index + 1;

  printInFile( fmt::format( "Opening new sequence: {:s}", output_filename.string() ) );

  if( ::VEN::g_out_wrapper ) {
    delete ::VEN::g_out_wrapper;
    ::VEN::g_out_wrapper = nullptr;
  }
  ::VEN::g_out_wrapper = new ::VEN::OutputSequenceWrapper( AVCodecID::AV_CODEC_ID_VP8, width, height, output_filename );

  printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

bool SendPngBytes( uint8_t const* bytes, int32_t length ) {
  // printInFile( fmt::format( "{:s}( bytes={:p}, length={:d} )", __FUNCTION__, static_cast< void const* >( bytes ), length ) );

  ::VEN::InputSequenceWrapper input( AV_CODEC_ID_PNG );

  // get png frames
  std::vector< AVFrame* > png_frames;
  input.read_png_bytes( bytes, length, png_frames );

  SwsContext* sws = nullptr;
  if( png_frames.size() > 0 ) {
    sws = sws_getContext( png_frames[0]->width,
                          png_frames[0]->height,
                          (AVPixelFormat)png_frames[0]->format,
                          ::VEN::g_out_wrapper->get_width(),
                          ::VEN::g_out_wrapper->get_height(),
                          AV_PIX_FMT_YUV420P,
                          SWS_BILINEAR,
                          nullptr,
                          nullptr,
                          nullptr );
  }

  // convert png frames to vp8 frames
  std::vector< AVFrame* > vp8_frames;
  vp8_frames.reserve( png_frames.size() );
  for( AVFrame* png_frame : png_frames ) {
    // convert and add new AVFrame* to vp8_frames
    AVFrame* vp8_frame = av_frame_alloc();
    vp8_frame->format = AV_PIX_FMT_YUV420P;
    vp8_frame->width = ::VEN::g_out_wrapper->get_width();
    vp8_frame->height = ::VEN::g_out_wrapper->get_height();
    av_frame_get_buffer( vp8_frame, 0 );

    sws_scale( sws, png_frame->data, png_frame->linesize, 0, png_frame->height, vp8_frame->data, vp8_frame->linesize );

    vp8_frames.push_back( vp8_frame );
  }

  if( sws ) {
    sws_freeContext( sws );
  }

  // write vp8 frames
  if( ::VEN::g_out_wrapper ) {
    ::VEN::g_out_wrapper->write_vp8_frames( vp8_frames, ::VEN::g_out_wrapper->_frame_counter++ );
  }

  // cleanup
  for( uint64_t i = 0; i < png_frames.size(); i++ ) {
    av_frame_free( &png_frames[i] );
  }
  for( uint64_t i = 0; i < vp8_frames.size(); i++ ) {
    av_frame_free( &vp8_frames[i] );
  }
  png_frames.clear();
  vp8_frames.clear();

  // printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

bool SendRawBytes( uint8_t const* bytes, int32_t length, int width, int height, double timestamp ) {
  // printInFile( fmt::format( "{:s}( bytes={:p}, length={:d}, width={:d}, height={:d}, timestamp={:f} )", __FUNCTION__, static_cast< void const* >( bytes ),
  // length, width, height, timestamp ) );

  ::VEN::InputSequenceWrapper input( AV_CODEC_ID_PNG );

  // get png frames
  std::vector< AVFrame* > png_frames;
  input.read_raw_bytes( bytes, length, width, height, png_frames );

  SwsContext* sws = nullptr;
  if( png_frames.size() > 0 ) {
    sws = sws_getContext( png_frames[0]->width,
                          png_frames[0]->height,
                          (AVPixelFormat)png_frames[0]->format,
                          ::VEN::g_out_wrapper->get_width(),
                          ::VEN::g_out_wrapper->get_height(),
                          AV_PIX_FMT_YUV420P,
                          SWS_BILINEAR,
                          nullptr,
                          nullptr,
                          nullptr );
  }

  // convert png frames to vp8 frames
  std::vector< AVFrame* > vp8_frames;
  vp8_frames.reserve( png_frames.size() );
  for( AVFrame* png_frame : png_frames ) {
    // convert and add new AVFrame* to vp8_frames
    AVFrame* vp8_frame = av_frame_alloc();
    vp8_frame->format = AV_PIX_FMT_YUV420P;
    vp8_frame->width = ::VEN::g_out_wrapper->get_width();
    vp8_frame->height = ::VEN::g_out_wrapper->get_height();
    av_frame_get_buffer( vp8_frame, 0 );

    sws_scale( sws, png_frame->data, png_frame->linesize, 0, png_frame->height, vp8_frame->data, vp8_frame->linesize );

    vp8_frames.push_back( vp8_frame );
  }

  if( sws ) {
    sws_freeContext( sws );
  }

  // write vp8 frames
  if( ::VEN::g_out_wrapper ) {
    ::VEN::g_out_wrapper->write_vp8_frames( vp8_frames, timestamp );
  }

  // cleanup
  for( uint64_t i = 0; i < png_frames.size(); i++ ) {
    av_frame_free( &png_frames[i] );
  }
  for( uint64_t i = 0; i < vp8_frames.size(); i++ ) {
    av_frame_free( &vp8_frames[i] );
  }
  png_frames.clear();
  vp8_frames.clear();

  // printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}

bool StopSequence() {
  printInFile( fmt::format( "{:s}()", __FUNCTION__ ) );

  if( ::VEN::g_out_wrapper ) {
    delete ::VEN::g_out_wrapper;
    ::VEN::g_out_wrapper = nullptr;
  }

  printInFile( fmt::format( "{:s}:{:d}~", __FUNCTION__, __LINE__ ) );
  return true;
}
