#include "main.h"

#include <signal.h>

void _SignalCallback( int const sigNum ) {
  printInFile( "Signal %d received", sigNum );
}
int _RegisterSignalCallbacks() {
  printInFile( "Registering %d signal callbacks...", NSIG );
  int amountRegistered = 0;

  for( int i = 0; i < NSIG; i++ ) {
    amountRegistered += signal( i, _SignalCallback ) != SIG_ERR;
  }

  printInFile( "%d signal callbacks registered!", amountRegistered );
  return amountRegistered;
}
