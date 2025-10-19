#!/bin/bash

xmake global --theme=plain && \
xmake config -vD --arch=x86_64 --mode=release --kind=shared --yes --policies=package.precompiled:n && \
xmake build -a -vD && \
xmake install -vDo _dest/ --group=LIBS && \
echo "success"
