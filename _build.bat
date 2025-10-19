@ECHO OFF

VERIFY OTHER 2>nul
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
IF NOT ERRORLEVEL 0 (
  echo Unable to enable extensions
)

SET "zigBin=D:\zig\zig.exe"

FOR /F "delims=" %%A IN ('cd') DO SET "ORIGINAL_DIR=%%A"
ECHO orig dir: %ORIGINAL_DIR%

SET "logFolder=.\_build_logs"

GOTO :main

:doCommand
SET "logFile=%logFolder%\%~1.log"
SET "command=%~2"
ECHO %command%>"%ORIGINAL_DIR%\%logFile%" 2>&1
%command%>>"%ORIGINAL_DIR%\%logFile%" 2>&1
EXIT /B %ERRORLEVEL%

:main

RMDIR /S /Q "%ORIGINAL_DIR%\%logFolder%"
RMDIR /S /Q "%ORIGINAL_DIR%\_dest"
RMDIR /S /Q "%ORIGINAL_DIR%\.xmake"
RMDIR /S /Q "%ORIGINAL_DIR%\build"

MKDIR "%ORIGINAL_DIR%\%logFolder%"

CALL :doCommand "00_made_build_logs" "echo we did it" && cd>NUL || Goto :END

CALL :doCommand "01_xmake_set_theme" "xmake global --theme=plain" && cd>NUL || Goto :END

CALL :doCommand "02_xmake_configure" "xmake config --import=.vscode\xmake.windows.shared.releasedbg.MD.conf -vD -y" && cd>NUL || Goto :END

CALL :doCommand "03_xmake_build" "xmake build -a -vD" && cd>NUL || Goto :END

CALL :doCommand "05_xmake_install" "xmake install -vDo _dest/ --group=LIBS" && cd>NUL || Goto :END

ECHO success

:END
cd "%ORIGINAL_DIR%"
ENDLOCAL
EXIT /B %ERRORLEVEL%
