#include "OfflinePacketCommunicator.h"

using namespace System;
using namespace PcapDotNet::Core;

PacketTotalStatistics^ OfflinePacketCommunicator::TotalStatistics::get()
{
    throw gcnew InvalidOperationException("Can't get TotalStatistics for offline devices");
}

OfflinePacketCommunicator::OfflinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth)
: PacketCommunicator(source, snapshotLength, flags, readTimeout, auth, nullptr)
{
}

void OfflinePacketCommunicator::Transmit(PacketSendQueue^ sendQueue, bool isSync)
{
    throw gcnew InvalidOperationException("Can't transmit queue to an offline device");
}
