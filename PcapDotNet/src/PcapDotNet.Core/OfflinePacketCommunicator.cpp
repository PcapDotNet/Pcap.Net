#include "OfflinePacketCommunicator.h"

using namespace System;
using namespace PcapDotNet::Core;

PacketTotalStatistics^ OfflinePacketCommunicator::TotalStatistics::get()
{
    throw gcnew InvalidOperationException("Can't get " + PacketTotalStatistics::typeid->Name + " for offline devices");
}

void OfflinePacketCommunicator::Transmit(PacketSendBuffer^, bool)
{
    throw gcnew InvalidOperationException("Can't transmit queue to an offline device");
}

OfflinePacketCommunicator::OfflinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth)
: PacketCommunicator(source, snapshotLength, attributes, readTimeout, auth, nullptr)
{
}
