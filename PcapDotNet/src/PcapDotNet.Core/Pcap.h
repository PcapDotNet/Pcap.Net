#pragma once

#define HAVE_REMOTE
#include <stdio.h>

#pragma warning(push)
#pragma warning(disable : 6386) // See https://connect.microsoft.com/VisualStudio/feedback/details/490105/c6386-in-ws2tcpip-h
#include <pcap.h>
#undef inline // Avoid macroizing keywords. C1189.
#pragma warning(pop)

#include <remote-ext.h>
#include <Win32-Extensions.h>