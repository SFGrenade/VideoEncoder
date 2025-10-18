#include "common.h"

void setCallback( LogCallback callback ) {
  g_log_message = callback;
}
void printInFile( std::string const &msg ) {
  if( g_log_message ) {
    g_log_message( msg.c_str() );
  }
}

#if defined( CM_Windows )
extern BOOL WINAPI DllMain( HINSTANCE const, DWORD const callReason, LPVOID const ) {
  switch( callReason ) {
    case DLL_PROCESS_ATTACH:
      break;
    case DLL_THREAD_ATTACH:
      break;
    case DLL_THREAD_DETACH:
      break;
    case DLL_PROCESS_DETACH:
      break;
  }
  return TRUE;
}
// extern BOOL WINAPI _DllMainCRTStartup( HINSTANCE const dllModHandle, DWORD const callReason, LPVOID const reserved ) {
//   return DllMain( dllModHandle, callReason, reserved );
// }
#endif
