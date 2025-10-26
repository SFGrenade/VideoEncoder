#include "main.h"

std::filesystem::path GS::mod_dir;
std::filesystem::path GS::save_dir;
std::atomic_uint64_t GS::sequence_index = 0;
std::string GS::file_extension = "mkv";
std::map< std::string, std::string > GS::codec_options;
std::map< std::string, std::string > GS::media_options;
std::list< std::shared_ptr< InputThread > > GS::input_threads;
