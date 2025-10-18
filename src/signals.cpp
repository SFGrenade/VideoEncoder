#include "main.h"

#include <signal.h>

#include "common.h"

void _SignalCallback( int const sigNum ) {
  printInFile( fmt::format( "Signal {:d} received", sigNum ) );
}
int _RegisterSignalCallbacks() {
  printInFile( fmt::format( "Registering {:d} signal callbacks...", NSIG ) );
  int amountRegistered = 0;

  for( int i = 0; i < NSIG; i++ ) {
    amountRegistered += signal( i, _SignalCallback ) != SIG_ERR;
  }

  printInFile( fmt::format( "{:d} signal callbacks registered!", amountRegistered ) );
  return amountRegistered;
}
