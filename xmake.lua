set_project( "VideoEncoder" )

set_version( "0.0.1", { build = "%Y%m%d", soname = true } )

add_rules( "mode.debug", "mode.release", "mode.releasedbg", "mode.minsizerel" )

set_languages( "c++20" )

if is_plat( "windows" ) then
    add_cxflags( "/permissive-" )
    add_cxflags( "/Zc:__cplusplus" )
    add_cxflags( "/Zc:preprocessor" )

    add_defines("_CRT_SECURE_NO_WARNINGS")
else
end

set_warnings( "allextra" )

-- maybe this helps for the ci?
set_policy( "build.across_targets_in_parallel", false )

if is_plat( "linux" ) then
    --add_requires( "glib" )
elseif is_plat( "macosx" ) then
    --add_requires( "lodepng" )
elseif is_plat( "windows" ) then
    --add_requires( "lodepng" )
else
end

--add_requireconfs( "**", "*.**", { system = false } )
--add_requireconfs( "*", { configs = { shared = false } } )

add_requires( "fmt", { alias = "fmt" } )
add_requireconfs( "fmt", { configs = { header_only = true } } )

target( "VideoEncoderNative" )
    set_kind( "shared" )

    set_default( true )
    set_group( "LIBS" )

    if is_plat( "linux" ) then
        -- maybe some platform-specific stuff
    elseif is_plat( "macosx" ) then
        -- maybe some platform-specific stuff
    elseif is_plat( "windows" ) then
        -- maybe some platform-specific stuff
    else
    end
    add_packages( "fmt", { public = true } )
    add_deps( "ffmpeg-helper", { public = true } )

    --add_includedirs( "src", { public = true } )
    add_includedirs( "src" )

    add_headerfiles( "src/*.h" )
    add_files( "src/*.cpp" )

    if is_plat( "linux" ) then
        add_files( "src/lin/*.cpp" )
    elseif is_plat( "macosx" ) then
        add_files( "src/mac/*.cpp" )
    elseif is_plat( "windows" ) then
        add_files( "src/win/*.cpp" )
    else
    end
target_end()

add_requires( "vcpkg::ffmpeg[avcodec,avformat,vpx,zlib]", { alias = "ffmpeg" } )
-- i love xmake+vcpkg
local ffmpeg_all_deps = { "libvpx", "zlib" }
for _, name in ipairs( ffmpeg_all_deps ) do
    add_requires("vcpkg::" .. name, { alias = name })
end

--add_requireconfs( "ffmpeg", { configs = { shared = false, features = { "all" } } } )
add_requireconfs( "ffmpeg", { configs = { shared = false, features = { "avcodec", "avformat", "vpx", "zlib" } } } )

target( "ffmpeg-helper" )
    set_kind( "phony" )
    --add_packages( "ffmpeg", "libiconv", "zlib", { public = true } )
    add_packages( "ffmpeg", { public = true } )
    for _, name in ipairs( ffmpeg_all_deps ) do
        add_packages( name, { public = true } )
    end
    if is_plat( "macosx", "iphoneos" ) then
        add_frameworks( "CoreFoundation", "Foundation", "CoreVideo", "CoreMedia", "VideoToolbox", "Security", { public = true } )
        if is_plat( "iphoneos" ) then
            add_frameworks( "AVFoundation", { public = true } )
        else
            add_frameworks( "AudioToolbox", { public = true } )
        end
    elseif is_plat( "linux" ) then
        add_arflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_asflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_cflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_cuflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_culdflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_culdflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_cxflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_cxxflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_dcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_fcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_gcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_kcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_ldflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_mflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_mrcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_mxflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_mxxflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_ncflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_pcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_rcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_scflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_shflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_zcflags( "-Wl,-Bsymbolic", { force = true, public = true } ) -- thanks to ffmpeg
        add_syslinks( "dl", "pthread", { public = true } )
    elseif is_plat( "android" ) then
        add_syslinks( "dl", "android", "mediandk", { public = true } )
    elseif is_plat( "windows" ) then
        add_syslinks( "Bcrypt", "Mfplat", "mfuuid", "Ole32", "Secur32", "Strmiids", "User32", "ws2_32", { public = true } )
    else
    end
target_end()
