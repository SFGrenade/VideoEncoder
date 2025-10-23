#include "main.h"

#include <mutex>
#include <thread>

#include "_ffmpeg.h"
#include "common.h"

InputData::InputData( uint8_t const* bytes, int32_t length ) : bytes( bytes, bytes + length ), width( 0 ), height( 0 ), timestamp( 0 ), is_raw( false ) {}

InputData::InputData( uint8_t const* bytes, int32_t length, int32_t width, int32_t height, double timestamp )
    : bytes( bytes, bytes + length ), width( width ), height( height ), timestamp( timestamp ), is_raw( true ) {}

InputThread::InputThread( OutputSequenceWrapper* out_wrapper ) : _out_wrapper( out_wrapper ) {
  auto runable = [this]() -> void { this->run(); };
  _thread = std::thread( runable );
}

InputThread::~InputThread() {
  end_input_thread();
  _thread.join();

  if( _out_wrapper ) {
    delete _out_wrapper;
    _out_wrapper = nullptr;
  }
}

void InputThread::end_input_thread() {
  should_stop = true;
}

void InputThread::receive_bytes( uint8_t const* bytes, int32_t length ) {
  std::lock_guard< std::mutex > guard( _mtx );
  _queue.push( InputData( bytes, length ) );
}

void InputThread::receive_bytes( uint8_t const* bytes, int32_t length, int32_t width, int32_t height, double timestamp ) {
  std::lock_guard< std::mutex > guard( _mtx );
  _queue.push( InputData( bytes, length, width, height, timestamp ) );
}

void InputThread::run() {
  while( !should_stop || !_queue.empty() ) {
    if( !_queue.empty() ) {
      _mtx.lock();
      InputData data = _queue.front();
      _queue.pop();
      _mtx.unlock();

      if( !data.is_raw ) {
        throw std::runtime_error( "Not implemented yet" );
      } else {
        process_raw_bytes( data.bytes, data.width, data.height, data.timestamp );
      }
    }
  }

  if( _out_wrapper ) {
    delete _out_wrapper;
    _out_wrapper = nullptr;
  }
}

void InputThread::process_png_bytes( std::vector< uint8_t > bytes ) {
  InputSequenceWrapper input( AV_CODEC_ID_PNG );

  // get png frames
  std::vector< AVFrame* > png_frames;
  input.read_png_bytes( bytes.data(), bytes.size(), png_frames );

  SwsContext* sws = nullptr;
  if( png_frames.size() > 0 ) {
    sws = sws_getContext( png_frames[0]->width,
                          png_frames[0]->height,
                          (AVPixelFormat)png_frames[0]->format,
                          _out_wrapper->get_width(),
                          _out_wrapper->get_height(),
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
    vp8_frame->width = _out_wrapper->get_width();
    vp8_frame->height = _out_wrapper->get_height();
    av_frame_get_buffer( vp8_frame, 0 );

    sws_scale( sws, png_frame->data, png_frame->linesize, 0, png_frame->height, vp8_frame->data, vp8_frame->linesize );

    vp8_frames.push_back( vp8_frame );
  }

  if( sws ) {
    sws_freeContext( sws );
  }

  // write vp8 frames
  _out_wrapper->write_vp8_frames( vp8_frames, _out_wrapper->_frame_counter++ );

  // cleanup
  for( uint64_t i = 0; i < png_frames.size(); i++ ) {
    av_frame_free( &png_frames[i] );
  }
  for( uint64_t i = 0; i < vp8_frames.size(); i++ ) {
    av_frame_free( &vp8_frames[i] );
  }
  png_frames.clear();
  vp8_frames.clear();
}

void InputThread::process_raw_bytes( std::vector< uint8_t > bytes, int width, int height, double timestamp ) {
  InputSequenceWrapper input( AV_CODEC_ID_PNG );

  // get png frames
  std::vector< AVFrame* > png_frames;
  input.read_raw_bytes( bytes.data(), bytes.size(), width, height, png_frames );

  SwsContext* sws = nullptr;
  if( png_frames.size() > 0 ) {
    sws = sws_getContext( png_frames[0]->width,
                          png_frames[0]->height,
                          (AVPixelFormat)png_frames[0]->format,
                          _out_wrapper->get_width(),
                          _out_wrapper->get_height(),
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
    vp8_frame->width = _out_wrapper->get_width();
    vp8_frame->height = _out_wrapper->get_height();
    av_frame_get_buffer( vp8_frame, 0 );

    sws_scale( sws, png_frame->data, png_frame->linesize, 0, png_frame->height, vp8_frame->data, vp8_frame->linesize );

    vp8_frames.push_back( vp8_frame );
  }

  if( sws ) {
    sws_freeContext( sws );
  }

  // write vp8 frames
  _out_wrapper->write_vp8_frames( vp8_frames, timestamp );

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
}
